namespace AppointmentProject.Models
{
    public class User
    {
        public int UserId { get; set; }  // Primary Key
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<PasswordReset> PasswordResets { get; set; }
        public ICollection<ActivityLog> ActivityLogs { get; set; }
    }
}
