using System.ComponentModel.DataAnnotations;

namespace AppointmentProject.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string ResetToken { get; set; }  // Hidden field to pass the reset token

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
