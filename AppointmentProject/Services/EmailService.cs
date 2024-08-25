using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AppointmentProject.Interfaces;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        // Define the sender's email and password for SMTP authentication
        string mail = "tutoriatseu-dev@outlook.com";
        string password = "Test123U5678!";

        // Create a new instance of SmtpClient and configure it
        var client = new SmtpClient("smtp.outlook.com")
        {
            Port = 587,
            EnableSsl = true, // Enable SSL for secure communication
            Credentials = new NetworkCredential(mail, password) // SMTP credentials
        };

        // Create a new MailMessage instance
        var mailMessage = new MailMessage
        {
            From = new MailAddress(mail), // Sender's email address
            To = { email }, // Recipient's email address
            Subject = subject, // Subject of the email
            Body = message, // Body of the email
            IsBodyHtml = true // Indicates that the email body is HTML
        };

        // Send the email asynchronously
        return client.SendMailAsync(mailMessage);
    }
}
