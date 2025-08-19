using Microsoft.EntityFrameworkCore;
using TravelDesk.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace TravelDesk.Data
{
    public class TravelDeskContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TravelDeskContext(DbContextOptions<TravelDeskContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TravelRequest> TravelRequests { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Removed seed data since Supabase database already has the data

            modelBuilder.Entity<User>()
                    .HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict); // Or NoAction

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // Or NoAction
            modelBuilder.Entity<User>()
                .HasOne(u => u.Manager)
                .WithMany()
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TravelRequest>()
                .HasOne(tr => tr.Project)
                .WithMany(p => p.TravelRequests)
                .HasForeignKey(tr => tr.ProjectId)
                .OnDelete(DeleteBehavior.Restrict); // Or NoAction
            modelBuilder.Entity<TravelRequest>()
                .Property(tr => tr.TicketUrl)
                .IsRequired(false); // Allow nulls
        }

        public override int SaveChanges()
        {
            ProcessAuditLogs();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ProcessAuditLogs();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ProcessAuditLogs()
        {
            var userId = GetCurrentUserId();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                AuditLog auditLog;

                // Try to get the Id property value safely
                var idProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id");
                if (idProperty == null) continue; // Skip if no 'Id' property found

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditLog = new AuditLog
                        {
                            UserId = userId,
                            EntityName = entry.Entity.GetType().Name,
                            ActionType = "EntityCreated",
                            Timestamp = DateTime.UtcNow,
                            EntityId = (int)(idProperty.CurrentValue ?? 0),
                            Changes = JsonSerializer.Serialize(entry.CurrentValues.ToObject())
                        };
                        break;
                    case EntityState.Modified:
                        var originalValues = new Dictionary<string, object>();
                        var currentValues = new Dictionary<string, object>();

                        foreach (var property in entry.Properties)
                        {
                            if (property.IsModified)
                            {
                                originalValues[property.Metadata.Name] = property.OriginalValue ?? "";
                                currentValues[property.Metadata.Name] = property.CurrentValue ?? "";
                            }
                        }
                        auditLog = new AuditLog
                        {
                            UserId = userId,
                            EntityName = entry.Entity.GetType().Name,
                            ActionType = "EntityModified",
                            Timestamp = DateTime.UtcNow,
                            EntityId = (int)(idProperty.CurrentValue ?? 0),
                            Changes = JsonSerializer.Serialize(new { Original = originalValues, Current = currentValues })
                        };
                        break;
                    case EntityState.Deleted:
                        auditLog = new AuditLog
                        {
                            UserId = userId,
                            EntityName = entry.Entity.GetType().Name,
                            ActionType = "EntityDeleted",
                            Timestamp = DateTime.UtcNow,
                            EntityId = (int)(idProperty.OriginalValue ?? 0),
                            Changes = JsonSerializer.Serialize(entry.OriginalValues.ToObject())
                        };
                        break;
                    default:
                        continue;
                }

                AuditLogs.Add(auditLog);
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            // Return a default or throw an exception if user ID cannot be determined
            // For now, returning 0 or a specific system user ID for unauthenticated actions
            return 0; 
        }
    }
}