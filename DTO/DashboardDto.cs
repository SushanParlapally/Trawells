using TravelDesk.Models;

namespace TravelDesk.DTO
{
    public class DashboardDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int TotalUsers { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalProjects { get; set; }
        public List<TravelRequestDto> Requests { get; set; } = new List<TravelRequestDto>();
    }
}
