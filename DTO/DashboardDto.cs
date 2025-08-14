using TravelDesk.Models;

namespace TravelDesk.DTO
{
    public class DashboardDto
    {
        public List<TravelRequestDto> Requests { get; set; } = new List<TravelRequestDto>();
    }

}
