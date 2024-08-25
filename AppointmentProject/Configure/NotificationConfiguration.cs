using AppointmentProject.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AppointmentProject.Configure
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Notifications-Appointment Relationship (Many-to-One)
            builder
                 .HasOne(n => n.Appointments)  // تعيين علاقة إلى كائن Appointment
                .WithMany(a => a.Notifications)  // تعيين مجموعة من Notifications
                .HasForeignKey(n => n.AppointmentId);  // تعيين المفتاح الخارجي
        }
    }
}
