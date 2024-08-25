using System.Threading.Tasks;

namespace AppointmentProject.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}