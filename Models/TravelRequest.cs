using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace TravelDesk.Models
{
    public class TravelRequest
    {
        public int TravelRequestId { get; set; }
       
        public int UserId { get; set; }
      
        public User? UserName { get; set; }
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public string ReasonForTravel { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string FromLocation { get; set; } = string.Empty;
        public string ToLocation { get; set; } = string.Empty;

        public string? Comments { get; set; }

        public string? TicketUrl { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsActive { get; set; }


    }
}
