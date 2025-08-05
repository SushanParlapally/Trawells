namespace TravelDesk.DTO
{
    public class UserActivityDto
    {
        public List<RecentRequestDto> RecentRequests { get; set; } = new();
        public List<ActiveUserDto> ActiveUsers { get; set; } = new();
    }

    public class RecentRequestDto
    {
        public int TravelRequestId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public UserInfoDto User { get; set; } = new();
        public string Department { get; set; } = string.Empty;
    }

    public class UserInfoDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ActiveUserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RequestCount { get; set; }
    }

    public class TeamMemberDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DepartmentDto Department { get; set; } = new();
        public RoleDto Role { get; set; } = new();
    }

    public class DepartmentDetailDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int UserCount { get; set; }
        public int RequestCount { get; set; }
    }

    public class ProjectDetailDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int RequestCount { get; set; }
        public List<RecentProjectRequestDto> RecentRequests { get; set; } = new();
    }

    public class RecentProjectRequestDto
    {
        public int TravelRequestId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public UserInfoDto User { get; set; } = new();
    }
} 