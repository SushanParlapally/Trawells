namespace TravelDesk.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }

        

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        public DepartmentDto Department { get; set; } = new DepartmentDto();
    }
}
