using TravelDesk.Models;

namespace TravelDesk.Services
{
    /// <summary>
    /// Interface for sending real-time notifications
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Send notification to a specific user
        /// </summary>
        Task SendToUserAsync(int userId, string type, string title, string message, object? data = null);

        /// <summary>
        /// Send notification to users with a specific role
        /// </summary>
        Task SendToRoleAsync(string roleName, string type, string title, string message, object? data = null);

        /// <summary>
        /// Send notification to users in a specific department
        /// </summary>
        Task SendToDepartmentAsync(int departmentId, string type, string title, string message, object? data = null);

        /// <summary>
        /// Send travel request status change notification
        /// </summary>
        Task SendTravelRequestNotificationAsync(TravelRequest request, string action, string performedBy);

        /// <summary>
        /// Send user management notification
        /// </summary>
        Task SendUserManagementNotificationAsync(int targetUserId, string action, string performedBy);
    }

    /// <summary>
    /// Notification data structure
    /// </summary>
    public class NotificationData
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public object? Data { get; set; }
        public string? ActionUrl { get; set; }
    }
}