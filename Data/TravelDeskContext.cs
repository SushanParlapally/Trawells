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


    }
    
}
