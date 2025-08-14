namespace TravelDesk.DTO
{
    public class TravelRequestDto
    {
        public int TravelRequestId { get; set; }
        public UserDto User { get; set; } = new UserDto();
        public ProjectDto Project { get; set; } = new ProjectDto();
        public string ReasonForTravel { get; set; } = string.Empty;
        
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string FromLocation { get; set; } = string.Empty;
        public string ToLocation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public string? TicketUrl { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
