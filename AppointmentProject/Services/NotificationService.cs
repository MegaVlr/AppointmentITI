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
                    var message = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333333;
        }}
        .header {{
            background-color: #f7f7f7;
            padding: 10px;
            text-align: center;
            border-bottom: 1px solid #dddddd;
        }}
        .content {{
            padding: 20px;
        }}
        .footer {{
            background-color: #f7f7f7;
            padding: 10px;
            text-align: center;
            border-top: 1px solid #dddddd;
            font-size: 0.9em;
            color: #777777;
        }}
        .appointment-details {{
            margin-top: 20px;
            padding: 10px;
            border: 1px solid #dddddd;
            background-color: #fafafa;
        }}
    </style>
</head>
<body>
    <div class='header'>
        <h2>Appointment Reminder</h2>
    </div>
    <div class='content'>
        <p>Dear {notification.Appointments.User.Name},</p>
        <p>This is a reminder that you have an upcoming appointment titled '<strong>{notification.Appointments.Title}</strong>', which is scheduled for <strong>{notification.Appointments.AppointmentDate:MMMM dd, yyyy 'at' hh:mm tt}</strong>.</p>
        <p>Please ensure to attend on time. Thank you.</p>

        <div class='appointment-details'>
            <h3>Appointment Details</h3>
            <p><strong>Title:</strong> {notification.Appointments.Title}</p>
            {(string.IsNullOrWhiteSpace(notification.Appointments.Description) ? "" : $"<p><strong>Description:</strong> {notification.Appointments.Description}</p>")}
            <p><strong>Date:</strong> {notification.Appointments.AppointmentDate:MMMM dd, yyyy}</p>
            <p><strong>Time:</strong> {notification.Appointments.AppointmentDate:hh:mm tt}</p>
        </div>

        <p>If you have any questions or need to reschedule, please don't hesitate to contact us.</p>
        <p>Best regards,<br>Moa3ydy</p>
    </div>
    <div class='footer'>
        <p><img src='https://cdn-icons-png.flaticon.com/512/5385/5385621.png' alt='Company Logo' style='width:150px;height:auto;'></p>
        <p>&copy; {DateTime.Now.Year} Moa3ydy. All rights reserved.</p>
    </div>
</body>
</html>
";


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
