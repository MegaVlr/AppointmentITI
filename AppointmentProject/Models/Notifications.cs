using System.Data;

namespace AppointmentProject.Models
{
    public class Notifications
    {
        public int  NotificationId { get; set; }
        public DateTime  Notification_Data_Time { get; set; }
        public bool  IsSent { get; set; }

    }
}
