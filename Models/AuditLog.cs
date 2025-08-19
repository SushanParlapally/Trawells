using System;
using System.ComponentModel.DataAnnotations;

namespace TravelDesk.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }        // The ID of the user who performed the action
        public required string ActionType { get; set; }  // e.g., "EntityCreated", "EntityModified"
        public required string EntityName { get; set; }  // e.g., "User", "TravelRequest"
        public int EntityId { get; set; }        // The Primary Key of the record that was changed
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Changes { get; set; }     // A JSON string detailing the changes
    }
}