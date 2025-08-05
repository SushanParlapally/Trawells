using Microsoft.EntityFrameworkCore;
using TravelDesk.Models;

namespace TravelDesk.Data
{
    public class TravelDeskContext : DbContext
    {
        public TravelDeskContext(DbContextOptions<TravelDeskContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
     
        public DbSet<Project > Projects { get; set; } 
        public DbSet<TravelRequest> TravelRequests { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {





            modelBuilder.Entity<Role>().HasData(
                 new Role { RoleId = 1, RoleName = "Admin", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true },
                 new Role { RoleId = 2, RoleName = "TravelAdmin", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true },
                 new Role { RoleId = 3, RoleName = "Manager", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true },
                 new Role { RoleId = 4, RoleName = "Employee", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true }
             );
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, DepartmentName = "IT", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true },
                new Department { DepartmentId = 2, DepartmentName = "HR", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true },
                new Department { DepartmentId = 3, DepartmentName = "Admin", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true },
                   new Department { DepartmentId = 4, DepartmentName = "Travel", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true }

                );

            modelBuilder.Entity<Project>().HasData(
      new Project { ProjectId = 1, ProjectName = "Project Alpha", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true },
      new Project { ProjectId = 2, ProjectName = "Project Beta", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true },
      new Project { ProjectId = 3, ProjectName = "Project Gamma", CreatedBy = 1, CreatedOn = DateTime.Now, IsActive = true }
  );

            // Seed default users for testing
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    UserId = 1, 
                    FirstName = "Admin", 
                    LastName = "User", 
                    Email = "admin@traveldesk.com", 
                    Password = "admin123", 
                    Address = "123 Admin Street", 
                    MobileNum = "1234567890", 
                    RoleId = 1, 
                    DepartmentId = 3, 
                    ManagerId = null, 
                    CreatedBy = 1, 
                    CreatedOn = DateTime.Now, 
                    IsActive = true 
                },
                new User 
                { 
                    UserId = 2, 
                    FirstName = "Travel", 
                    LastName = "Admin", 
                    Email = "traveladmin@traveldesk.com", 
                    Password = "travel123", 
                    Address = "456 Travel Street", 
                    MobileNum = "2345678901", 
                    RoleId = 2, 
                    DepartmentId = 4, 
                    ManagerId = 1, 
                    CreatedBy = 1, 
                    CreatedOn = DateTime.Now, 
                    IsActive = true 
                },
                new User 
                { 
                    UserId = 3, 
                    FirstName = "Manager", 
                    LastName = "User", 
                    Email = "manager@traveldesk.com", 
                    Password = "manager123", 
                    Address = "789 Manager Street", 
                    MobileNum = "3456789012", 
                    RoleId = 3, 
                    DepartmentId = 1, 
                    ManagerId = 1, 
                    CreatedBy = 1, 
                    CreatedOn = DateTime.Now, 
                    IsActive = true 
                },
                new User 
                { 
                    UserId = 4, 
                    FirstName = "Employee", 
                    LastName = "User", 
                    Email = "employee@traveldesk.com", 
                    Password = "employee123", 
                    Address = "321 Employee Street", 
                    MobileNum = "4567890123", 
                    RoleId = 4, 
                    DepartmentId = 1, 
                    ManagerId = 3, 
                    CreatedBy = 1, 
                    CreatedOn = DateTime.Now, 
                    IsActive = true 
                }
            );

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


    }
    
}
