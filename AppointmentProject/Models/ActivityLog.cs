namespace AppointmentProject.Models
{
    public class ActivityLog
    {
        public int ActivityLogId { get; set; }  // Primary Key
        public int UserId { get; set; }  // Foreign Key referencing User
        public string?Action { get; set; }  // Description of the user action
        public DateTime? Timestamp { get; set; }  // Time when the action occurred
        // Navigation property
        public User User { get; set; }  // Reference to the User who performed the action
    }

}
