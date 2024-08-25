namespace AppointmentProject.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }  // Primary Key
        public int UserId { get; set; }  // Foreign Key referencing User
        public DateTime AppointmentDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public string ? Description { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation property
        public User User { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}