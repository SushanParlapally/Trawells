namespace TravelDesk.DTO
{
    public class TravelRequestDto
    {
        public int TravelRequestId { get; set; }
        public UserDto User { get; set; }
        public ProjectDto Project { get; set; }
        public string ReasonForTravel { get; set; }
        
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public string Status { get; set; }
        public string? Comments { get; set; }
        public string? TicketUrl { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
