using Microsoft.AspNetCore.SignalR;
using TravelDesk.Hubs;
using TravelDesk.Models;
using TravelDesk.Data;
using Microsoft.EntityFrameworkCore;

namespace TravelDesk.Services
{
    /// <summary>
    /// Service for sending real-time notifications via SignalR
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly TravelDeskContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            TravelDeskContext context,
            ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Send notification to a specific user
        /// </summary>
        public async Task SendToUserAsync(int userId, string type, string title, string message, object? data = null)
        {
            try
            {
                var notification = new NotificationData
                {
                    Id = Random.Shared.Next(1000, 9999), // In real app, this would be from database
                    Type = type,
                    Title = title,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    IsRead = false,
                    Data = data
                };

                await _hubContext.Clients.Group($"User_{userId}")
                    .SendAsync("ReceiveNotification", notification);

                _logger.LogInformation("Notification sent to user {UserId}: {Title}", userId, title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to user {UserId}", userId);
            }
        }

        /// <summary>
        /// Send notification to users with a specific role
        /// </summary>
        public async Task SendToRoleAsync(string roleName, string type, string title, string message, object? data = null)
        {
            try
            {
                var notification = new NotificationData
                {
                    Id = Random.Shared.Next(1000, 9999),
                    Type = type,
                    Title = title,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    IsRead = false,
                    Data = data
                };

                await _hubContext.Clients.Group($"Role_{roleName}")
                    .SendAsync("ReceiveNotification", notification);

                _logger.LogInformation("Notification sent to role {RoleName}: {Title}", roleName, title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to role {RoleName}", roleName);
            }
        }

        /// <summary>
        /// Send notification to users in a specific department
        /// </summary>
        public async Task SendToDepartmentAsync(int departmentId, string type, string title, string message, object? data = null)
        {
            try
            {
                // Get all users in the department
                var userIds = await _context.Users
                    .Where(u => u.DepartmentId == departmentId && u.IsActive)
                    .Select(u => u.UserId)
                    .ToListAsync();

                var notification = new NotificationData
                {
                    Id = Random.Shared.Next(1000, 9999),
                    Type = type,
                    Title = title,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    IsRead = false,
                    Data = data
                };

                // Send to each user in the department
                foreach (var userId in userIds)
                {
                    await _hubContext.Clients.Group($"User_{userId}")
                        .SendAsync("ReceiveNotification", notification);
                }

                _logger.LogInformation("Notification sent to department {DepartmentId}: {Title}", departmentId, title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to department {DepartmentId}", departmentId);
            }
        }

        /// <summary>
        /// Send travel request status change notification
        /// </summary>
        public async Task SendTravelRequestNotificationAsync(TravelRequest request, string action, string performedBy)
        {
            try
            {
                var title = action switch
                {
                    "approved" => "Travel Request Approved",
                    "rejected" => "Travel Request Rejected",
                    "returned" => "Travel Request Returned",
                    "booked" => "Travel Booking Confirmed",
                    "completed" => "Travel Request Completed",
                    _ => "Travel Request Updated"
                };

                var message = action switch
                {
                    "approved" => $"Your travel request #{request.TravelRequestId} to {request.ToLocation} has been approved by {performedBy}",
                    "rejected" => $"Your travel request #{request.TravelRequestId} to {request.ToLocation} has been rejected by {performedBy}",
                    "returned" => $"Your travel request #{request.TravelRequestId} to {request.ToLocation} has been returned by {performedBy} for revision",
                    "booked" => $"Your travel to {request.ToLocation} has been booked by {performedBy}",
                    "completed" => $"Your travel request #{request.TravelRequestId} to {request.ToLocation} has been completed",
                    _ => $"Your travel request #{request.TravelRequestId} has been updated by {performedBy}"
                };

                var notificationData = new
                {
                    requestId = request.TravelRequestId,
                    status = request.Status,
                    destination = request.ToLocation,
                    actionUrl = $"/employee/requests/{request.TravelRequestId}"
                };

                // Send to the employee who created the request
                await SendToUserAsync(request.UserId, "travel_request", title, message, notificationData);

                // If approved, also notify travel admin
                if (action == "approved")
                {
                    await SendToRoleAsync("TravelAdmin", "travel_request", 
                        "New Request for Booking", 
                        $"Travel request #{request.TravelRequestId} by {request.UserName?.FirstName} {request.UserName?.LastName} needs booking",
                        notificationData);
                }

                _logger.LogInformation("Travel request notification sent for request {RequestId}: {Action}", request.TravelRequestId, action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send travel request notification for request {RequestId}", request.TravelRequestId);
            }
        }

        /// <summary>
        /// Send user management notification
        /// </summary>
        public async Task SendUserManagementNotificationAsync(int targetUserId, string action, string performedBy)
        {
            try
            {
                var title = action switch
                {
                    "created" => "Account Created",
                    "updated" => "Account Updated",
                    "deactivated" => "Account Deactivated",
                    "role_changed" => "Role Changed",
                    _ => "Account Modified"
                };

                var message = action switch
                {
                    "created" => $"Your account has been created by {performedBy}",
                    "updated" => $"Your account information has been updated by {performedBy}",
                    "deactivated" => $"Your account has been deactivated by {performedBy}",
                    "role_changed" => $"Your role has been changed by {performedBy}",
                    _ => $"Your account has been modified by {performedBy}"
                };

                await SendToUserAsync(targetUserId, "user_management", title, message);

                _logger.LogInformation("User management notification sent to user {UserId}: {Action}", targetUserId, action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send user management notification to user {UserId}", targetUserId);
            }
        }
    }
}