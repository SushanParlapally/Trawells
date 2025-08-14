using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TravelDesk.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        [Required]
        [StringLength(50)]
        public string DepartmentName { get; set; } = string.Empty;
        [JsonIgnore]

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<TravelRequest> TravelRequests { get; set; } = new List<TravelRequest>();
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
