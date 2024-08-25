using System.Data;

namespace AppointmentProject.Models
{
    public class Notification
    {
        public int  NotificationId { get; set; }// Primary Key
        public int AppointmentId { get; set; }  // Foreign Key referencing User
        public DateTime?  Notification_Data_Time { get; set; }
        public bool  IsSent { get; set; }

        // Navigation properties
            public Appointment Appointments { get; set; }  // Reference to the Appointment who requested the reset

    }
}
