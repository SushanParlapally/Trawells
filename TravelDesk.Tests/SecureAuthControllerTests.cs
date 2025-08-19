using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelDesk.Data;
using TravelDesk.DTOs;
using TravelDesk.Models;
using TravelDesk.Services;
using Xunit;

namespace TravelDesk.Tests
{
    /// <summary>
    /// Integration tests for SecureAuthController to ensure proper authentication flow
    /// </summary>
    public class SecureAuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public SecureAuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<TravelDeskContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add in-memory database for testing
                    services.AddDbContext<TravelDeskContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_ValidUser_ReturnsCreated()
        {
            // Arrange
            await SeedTestData();
            
            var userCreateDto = new UserCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@test.com",
                MobileNum = "1234567890",
                Address = "123 Test Street, Test City",
                Password = "SecurePass123!",
                RoleId = 1,
                DepartmentId = 1
            };

            var json = JsonSerializer.Serialize(userCreateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/secureauth/register", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("User created successfully", responseContent);
            Assert.Contains("john.doe@test.com", responseContent);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsConflict()
        {
            // Arrange
            await SeedTestData();
            
            var userCreateDto = new UserCreateDto
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "existing@test.com", // This email already exists in seed data
                MobileNum = "1234567890",
                Address = "123 Test Street, Test City",
                Password = "SecurePass123!",
                RoleId = 1,
                DepartmentId = 1
            };

            var json = JsonSerializer.Serialize(userCreateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/secureauth/register", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("A user with this email already exists", responseContent);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            await SeedTestData();
            
            var loginDto = new LoginDto
            {
                Email = "existing@test.com",
                Password = "TestPassword123!"
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/secureauth/login", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("token", responseContent);
            Assert.Contains("existing@test.com", responseContent);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            await SeedTestData();
            
            var loginDto = new LoginDto
            {
                Email = "existing@test.com",
                Password = "WrongPassword123!"
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/secureauth/login", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Invalid email or password", responseContent);
        }

        [Fact]
        public async Task Login__ReturnsUnauthorized()
        {
            // Arrange
            await SeedTestData();
            
            var loginDto = new LoginDto
            {
                Email = "nonexistent@test.com",
                Password = "TestPassword123!"
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/secureauth/login", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Invalid email or password", responseContent);
        }

        [Theory]
        [InlineData("", "password")]
        [InlineData("invalid-email", "password")]
        [InlineData("test@test.com", "")]
        public async Task Login_InvalidInput_ReturnsBadRequest(string email, string password)
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/secureauth/login", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("", "Doe", "invalid-email", "1234567890", "123 Test Street", "short", 1, 1)] // Invalid email, short password
        [InlineData("John", "", "john.doe@test.com", "123", "123 Test Street", "SecurePass123!", 1, 1)] // Short mobile, empty last name
        [InlineData("J", "Doe", "john.doe@test.com", "1234567890", "short", "SecurePass123!", 1, 1)] // Short first name, short address
        [InlineData("John", "Doe", "john.doe@test.com", "1234567890", "123 Test Street", "weakpass", 0, 0)] // Weak password, invalid role/department
        public async Task Register_InvalidInput_ReturnsBadRequest(
            string firstName, string lastName, string email, string mobileNum, string address, string password, int roleId, int departmentId)
        {
            // Arrange
            var userCreateDto = new UserCreateDto
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                MobileNum = mobileNum,
                Address = address,
                Password = password,
                RoleId = roleId,
                DepartmentId = departmentId
            };

            var json = JsonSerializer.Serialize(userCreateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/secureauth/register", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        private async Task SeedTestData()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TravelDeskContext>();
            var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

            // Clear existing data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed test data
            var role = new Role { RoleId = 1, RoleName = "Employee" };
            var department = new Department { DepartmentId = 1, DepartmentName = "IT" };
            
            context.Roles.Add(role);
            context.Departments.Add(department);

            var existingUser = new User
            {
                UserId = 1,
                FirstName = "Existing",
                LastName = "User",
                Email = "existing@test.com",
                MobileNum = "1234567890",
                Address = "123 Existing Street",
                PasswordHash = passwordService.HashPassword("TestPassword123!"),
                RoleId = 1,
                DepartmentId = 1,
                IsActive = true,
                CreatedOn = DateTime.UtcNow
            };

            context.Users.Add(existingUser);
            await context.SaveChangesAsync();
        }
    }
}