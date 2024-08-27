using Microsoft.AspNetCore.Mvc;
using AppointmentProject.Models;
using System.Linq;
using System;
using AppointmentProject.Models.ViewModels;
using AppointmentProject.Data;
using AppointmentProject.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppointmentProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppointmentDbContext _context;
        private readonly IEmailService _emailService; // Email service for sending emails
        private readonly IConfiguration _configuration;

        public AccountController(AppointmentDbContext context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            var model = new ForgotPasswordViewModel();
            return View(model);
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

                    // Success message - email sent
                    ViewData["Message"] = "An email with a password reset link has been sent to your email address. Please check your inbox.";
                    return View(model);
                }
                else
                {
                    // Email not found
                    ViewData["Message"] = "The email address you entered is not associated with any account.";
                }
            }
            else
            {
                // Model state is invalid
                ViewData["Message"] = "Please enter a valid email address.";
            }

            // Return the view with the message
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

                        // Optionally set a success message in ViewData if you want to show it on the view
                        ViewData["SuccessMessage"] = "Your password has been reset successfully.";
                        return RedirectToAction("ResetPasswordConfirmation");
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid token or token expired.");
                }
            }
            // Return the view with the model and any validation errors
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

        // GET: /Account/SignUp
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: /Account/SignUp
        [HttpPost]
        public IActionResult SignUp(string name, string email, string password,string PhoneNumber)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(PhoneNumber))
            {
                ViewBag.Message = "All fields are required.";
                return View();
            }

            // Check if email already exists
            var existingUser = _context.Users.SingleOrDefault(u => u.Email == email);
            var existingPhoneNumber = _context.Users.SingleOrDefault(p => p.phoneNumber == PhoneNumber);
            if (existingUser != null)
            {
                ViewBag.Message = "Email already exists.";
                return View();
            }
            if (existingPhoneNumber != null)
            {
                ViewBag.Message = "PhoneNumber already exists.";
                return View();
            }

            // Hash the password
            var hashedPassword = HashPassword(password);

            // Create new user
            var newUser = new User
            {
                Name = name,
                Email = email,
                PasswordHash = hashedPassword,
                phoneNumber = PhoneNumber,
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            ViewBag.Message = "Registration successful!";
            return View();
        }

        // GET: /Account/SignIn
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        // POST: /Account/SignIn
        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewData["ErrorMessage"] = "All inputs required";
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                // Generate token
                var token = GenerateJwtToken(user);
                Response.Cookies.Append("authToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(1) // Token expiration time
                });

                // Store token in ViewData to be accessed by JavaScript
                ViewData["Token"] = token;

                return RedirectToAction("Index", "Home");
            }

            ViewData["ErrorMessage"] = "Incorrect email or password";
            return View();
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet]
        public IActionResult Logout()
        {
            // Remove the authToken cookie
            Response.Cookies.Delete("authToken");
            // Redirect to SignIn page
            return RedirectToAction("SignIn", "Account");
        }
    }
}



