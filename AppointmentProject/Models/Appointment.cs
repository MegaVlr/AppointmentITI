namespace AppointmentProject.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }  // Primary Key
        public int UserId { get; set; }  // Foreign Key referencing User
        public DateTime AppointmentDate { get; set; }
        public string Description { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}