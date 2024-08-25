using AppointmentProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentProject.Data
{
    public class AppointmentDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User-Appointment Relationship (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Appointments)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

            // User-PasswordReset Relationship (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.PasswordResets)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            // ActivityLog-User Relationship (Many-to-One)
            modelBuilder.Entity<ActivityLog>()
                .HasOne(al => al.User)
                .WithMany(u => u.ActivityLogs)
                .HasForeignKey(al => al.UserId);
        }
    }
}
