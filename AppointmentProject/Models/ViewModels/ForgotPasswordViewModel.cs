using System.ComponentModel.DataAnnotations;

namespace AppointmentProject.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required] // ensures that the Email field is not left empty
        [EmailAddress] // ensures that the Email field contains a valid email address
        public string Email { get; set; }
    }
}
