using System.ComponentModel.DataAnnotations;

namespace TravelDesk.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new user
    /// </summary>
    public class UserCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 10)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string MobileNum { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public int? ManagerId { get; set; }
    }
}