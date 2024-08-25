namespace AppointmentProject.Models
{
    public class PasswordReset
    {
        public int PasswordResetId { get; set; }  // Primary Key
        public int UserId { get; set; }  // Foreign Key referencing User
        public string ResetToken { get; set; }  // Token used to reset password
        public DateTime RequestTime { get; set; }  // Time when the reset request was made
        public bool IsUsed { get; set; }  // Indicates whether the reset token has been used

        // Navigation property
        public User User { get; set; }  // Reference to the User who requested the reset
    }

}
