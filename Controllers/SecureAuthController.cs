using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TravelDesk.Data;
using TravelDesk.DTOs;
using TravelDesk.Services;
using FluentValidation;

namespace TravelDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecureAuthController : ControllerBase
    {
        private readonly TravelDeskContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<UserCreateDto> _userCreateValidator;
        private readonly ILogger<SecureAuthController> _logger;

        public SecureAuthController(
            TravelDeskContext context,
            IPasswordService passwordService,
            IConfiguration configuration,
            IValidator<LoginDto> loginValidator,
            IValidator<UserCreateDto> userCreateValidator,
            ILogger<SecureAuthController> logger)
        {
            _context = context;
            _passwordService = passwordService;
            _configuration = configuration;
            _loginValidator = loginValidator;
            _userCreateValidator = userCreateValidator;
            _logger = logger;
        }

        /// <summary>
        /// Secure login endpoint with proper password verification
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Validate input
                var validationResult = await _loginValidator.ValidateAsync(loginDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { 
                        message = "Invalid input", 
                        errors = validationResult.Errors.Select(e => e.ErrorMessage) 
                    });
                }

                // Find user by email only (never query by password)
                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", loginDto.Email);
                    // Return generic error to prevent email enumeration
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Verify password using BCrypt
                if (!_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for user: {Email}", loginDto.Email);
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                _logger.LogInformation("Successful login for user: {Email}", loginDto.Email);

                return Ok(new
                {
                    token = token,
                    user = new
                    {
                        userId = user.UserId,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        role = user.Role?.RoleName,
                        department = user.Department?.DepartmentName
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Secure user registration endpoint with password hashing
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto userCreateDto)
        {
            try
            {
                // Validate input
                var validationResult = await _userCreateValidator.ValidateAsync(userCreateDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { 
                        message = "Invalid input", 
                        errors = validationResult.Errors.Select(e => e.ErrorMessage) 
                    });
                }

                // Check if email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == userCreateDto.Email);

                if (existingUser != null)
                {
                    return Conflict(new { message = "A user with this email already exists" });
                }

                // Verify role and department exist
                var roleExists = await _context.Roles.AnyAsync(r => r.RoleId == userCreateDto.RoleId);
                var departmentExists = await _context.Departments.AnyAsync(d => d.DepartmentId == userCreateDto.DepartmentId);

                if (!roleExists)
                {
                    return BadRequest(new { message = "Invalid role selected" });
                }

                if (!departmentExists)
                {
                    return BadRequest(new { message = "Invalid department selected" });
                }

                // Verify manager exists if provided
                if (userCreateDto.ManagerId.HasValue)
                {
                    var managerExists = await _context.Users
                        .AnyAsync(u => u.UserId == userCreateDto.ManagerId.Value && u.IsActive);
                    
                    if (!managerExists)
                    {
                        return BadRequest(new { message = "Invalid manager selected" });
                    }
                }

                // Hash the password
                var passwordHash = _passwordService.HashPassword(userCreateDto.Password);

                // Create new user
                var user = new User
                {
                    FirstName = userCreateDto.FirstName,
                    LastName = userCreateDto.LastName,
                    Address = userCreateDto.Address,
                    Email = userCreateDto.Email,
                    MobileNum = userCreateDto.MobileNum,
                    PasswordHash = passwordHash, // Store hashed password
                    RoleId = userCreateDto.RoleId,
                    DepartmentId = userCreateDto.DepartmentId,
                    ManagerId = userCreateDto.ManagerId,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("New user registered: {Email}", user.Email);

                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new
                {
                    userId = user.UserId,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    message = "User created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for email: {Email}", userCreateDto.Email);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }

        /// <summary>
        /// Get user by ID (for CreatedAtAction)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                userId = user.UserId,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                role = user.Role?.RoleName,
                department = user.Department?.DepartmentName
            });
        }

        /// <summary>
        /// Generates a JWT token for the authenticated user
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "default-secret-key-for-development-if-not-set";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                new Claim("DepartmentId", user.DepartmentId.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}