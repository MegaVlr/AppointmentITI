using AppointmentProject.Configure;
using AppointmentProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentProject.Data
{
    public class AppointmentDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Create Schema
            modelBuilder.HasDefaultSchema("Appo");
            // User Configuration
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            // ActivityLog Configuration
            modelBuilder.ApplyConfiguration(new ActivityLogConfiguration());
            // Appointment Configuration 
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            // Notification Configuration 
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            // PasswordReset Configuration 
            modelBuilder.ApplyConfiguration(new PasswordResetConfiguration());
        }
    }
}
