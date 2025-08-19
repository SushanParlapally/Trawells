using System;
using TravelDesk.Services;
using Xunit;

namespace TravelDesk.Tests
{
    /// <summary>
    /// Unit tests for PasswordService to ensure secure password hashing and verification
    /// </summary>
    public class PasswordServiceTests
    {
        private readonly IPasswordService _passwordService;

        public PasswordServiceTests()
        {
            _passwordService = new PasswordService();
        }

        [Fact]
        public void HashPassword_ValidPassword_ReturnsHashedPassword()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hashedPassword = _passwordService.HashPassword(password);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEqual(password, hashedPassword);
            Assert.True(hashedPassword.Length > 50); // BCrypt hashes are typically 60 characters
        }

        [Fact]
        public void HashPassword_SamePassword_ReturnsDifferentHashes()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash1 = _passwordService.HashPassword(password);
            var hash2 = _passwordService.HashPassword(password);

            // Assert
            Assert.NotEqual(hash1, hash2); // BCrypt uses salt, so same password should produce different hashes
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "TestPassword123!";
            var hashedPassword = _passwordService.HashPassword(password);

            // Act
            var result = _passwordService.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var correctPassword = "TestPassword123!";
            var incorrectPassword = "WrongPassword456!";
            var hashedPassword = _passwordService.HashPassword(correctPassword);

            // Act
            var result = _passwordService.VerifyPassword(incorrectPassword, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void HashPassword_InvalidPassword_ThrowsArgumentException(string invalidPassword)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _passwordService.HashPassword(invalidPassword));
        }

        [Theory]
        [InlineData("", "validhash")]
        [InlineData(" ", "validhash")]
        [InlineData(null, "validhash")]
        [InlineData("validpassword", "")]
        [InlineData("validpassword", " ")]
        [InlineData("validpassword", null)]
        public void VerifyPassword_InvalidInputs_ThrowsArgumentException(string password, string hash)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _passwordService.VerifyPassword(password, hash));
        }

        [Fact]
        public void VerifyPassword_MalformedHash_ReturnsFalse()
        {
            // Arrange
            var password = "TestPassword123!";
            var malformedHash = "this-is-not-a-valid-bcrypt-hash";

            // Act
            var result = _passwordService.VerifyPassword(password, malformedHash);

            // Assert
            Assert.False(result); // Should return false instead of throwing exception
        }

        [Fact]
        public void HashPassword_LongPassword_HandlesCorrectly()
        {
            // Arrange
            var longPassword = new string('a', 100); // 100 character password

            // Act
            var hashedPassword = _passwordService.HashPassword(longPassword);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.True(_passwordService.VerifyPassword(longPassword, hashedPassword));
        }

        [Fact]
        public void HashPassword_SpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var passwordWithSpecialChars = "P@ssw0rd!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var hashedPassword = _passwordService.HashPassword(passwordWithSpecialChars);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.True(_passwordService.VerifyPassword(passwordWithSpecialChars, hashedPassword));
        }
    }
}