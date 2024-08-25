using Microsoft.AspNetCore.Mvc;
using AppointmentProject.Models;
using System.Linq;
using System;
using AppointmentProject.Models.ViewModels;
using AppointmentProject.Data;
using AppointmentProject.Interfaces;
using System.Threading.Tasks;

namespace AppointmentProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppointmentDbContext _context;
        private readonly IEmailService _emailService; // Email service for sending emails

        public AccountController(AppointmentDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    // Generate the reset token
                    var resetToken = Guid.NewGuid().ToString();

                    // Create a PasswordReset entity
                    var passwordReset = new PasswordReset
                    {
                        UserId = user.UserId,
                        ResetToken = resetToken,
                        RequestTime = DateTime.UtcNow,
                        IsUsed = false
                    };
                    _context.PasswordResets.Add(passwordReset);
                    await _context.SaveChangesAsync(); // Use async save method

                    // Generate reset link
                    var resetLink = Url.Action("ResetPassword", "Account",
                        new { token = resetToken }, protocol: HttpContext.Request.Scheme);

                    // Send email with the reset link
                    await _emailService.SendEmailAsync(user.Email, "Password Reset Request",
                        $"Please reset your password by clicking <a href='{resetLink}'>here</a>.");

                    return RedirectToAction("ForgotPasswordConfirmation");
                }
                else
                {
                    ModelState.AddModelError("", "Email not found.");
                }
            }

            return View(model);
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("A token is required for password reset.");
            }

            var model = new ResetPasswordViewModel { ResetToken = token };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var resetEntry = _context.PasswordResets
                    .SingleOrDefault(r => r.ResetToken == model.ResetToken && !r.IsUsed);

                if (resetEntry != null && resetEntry.RequestTime.AddHours(1) >= DateTime.UtcNow)
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserId == resetEntry.UserId);
                    if (user != null)
                    {
                        user.PasswordHash = HashPassword(model.NewPassword);
                        resetEntry.IsUsed = true;
                        await _context.SaveChangesAsync(); // Use async save method

                        return RedirectToAction("ResetPasswordConfirmation");
                    }
                }

                ModelState.AddModelError("", "Invalid token or token expired.");
            }

            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private string HashPassword(string password)
        {
            // Password hashing logic
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
