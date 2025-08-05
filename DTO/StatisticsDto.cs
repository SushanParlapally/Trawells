namespace TravelDesk.DTO
{
    public class StatisticsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int BookedRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int ReturnedRequests { get; set; }
        public List<DepartmentStatisticsDto> RequestsByDepartment { get; set; } = new();
        public List<StatusStatisticsDto> RecentRequestsByStatus { get; set; } = new();
    }

    public class DepartmentStatisticsDto
    {
        public string Department { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class StatusStatisticsDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class UserStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int Managers { get; set; }
        public int Employees { get; set; }
        public int TravelAdmins { get; set; }
        public List<DepartmentStatisticsDto> UsersByDepartment { get; set; } = new();
    }

    public class SystemStatisticsDto
    {
        public int Departments { get; set; }
        public int Projects { get; set; }
    }

    public class AdminStatisticsDto
    {
        public UserStatisticsDto Users { get; set; } = new();
        public StatisticsDto TravelRequests { get; set; } = new();
        public SystemStatisticsDto System { get; set; } = new();
    }

    public class ManagerStatisticsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int TeamMembersCount { get; set; }
        public List<StatusStatisticsDto> RecentRequestsByStatus { get; set; } = new();
        public List<DepartmentStatisticsDto> RequestsByDepartment { get; set; } = new();
    }

    public class DepartmentStatisticsResponseDto
    {
        public int TotalDepartments { get; set; }
        public int ActiveDepartments { get; set; }
        public List<UserCountDto> UsersByDepartment { get; set; } = new();
        public List<RequestCountDto> RequestsByDepartment { get; set; } = new();
    }

    public class UserCountDto
    {
        public string Department { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }

    public class RequestCountDto
    {
        public string Department { get; set; } = string.Empty;
        public int RequestCount { get; set; }
    }

    public class ProjectStatisticsDto
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public List<ProjectRequestCountDto> RequestsByProject { get; set; } = new();
        public List<TopProjectDto> TopProjects { get; set; } = new();
    }

    public class ProjectRequestCountDto
    {
        public string Project { get; set; } = string.Empty;
        public int RequestCount { get; set; }
    }

    public class TopProjectDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int RequestCount { get; set; }
    }
} 