using AppointmentProject.Data;
using AppointmentProject.Interfaces;
using AppointmentProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class NotificationService
{
    private readonly AppointmentDbContext _context;
    private readonly IEmailService _emailService;

    public NotificationService(AppointmentDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task SendPendingNotificationsAsync()
    {
        // Fetch all pending notifications that are due to be sent
        var pendingNotifications = await _context.Notifications
            .Include(n => n.Appointments) // Include related appointment data
            .ThenInclude(a => a.User)     // Include the related User data in the Appointment
            .Where(n => !n.IsSent && n.Notification_Data_Time <= DateTime.Now)
            .ToListAsync();

        foreach (var notification in pendingNotifications)
        {
            try
            {
                // Ensure that the User object is not null
                if (notification.Appointments.User != null)
                {
                    // Compose the email content
                    var subject = "Appointment Reminder";
                    var message = $"Dear {notification.Appointments.User.Name}, you have an upcoming appointment: {notification.Appointments.Title} scheduled on {notification.Appointments.AppointmentDate}.";

                    // Send the email
                    await _emailService.SendEmailAsync(notification.Appointments.User.Email, subject, message);

                    // Mark the notification as sent
                    notification.IsSent = true;
                    _context.Notifications.Update(notification);
                }
                else
                {
                    Console.WriteLine("User information is missing for the appointment. Email not sent.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        // Save changes to the database
        await _context.SaveChangesAsync();
    }

}
