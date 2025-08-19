using System.ComponentModel.DataAnnotations;

namespace TravelDesk.DTOs
{
    /// <summary>
    /// Data Transfer Object for user login
    /// </summary>
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Password { get; set; } = string.Empty;
    }
}