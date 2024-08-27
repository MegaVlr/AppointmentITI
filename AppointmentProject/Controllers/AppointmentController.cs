using AppointmentProject.Data;
using AppointmentProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace AppointmentProject.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppointmentDbContext _context;
        private readonly IConfiguration _configuration;

        public AppointmentController(AppointmentDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        // GET: /Appointment/Appointment
        [HttpGet]
        public IActionResult Appointment()
        {
            var token = Request.Cookies["authToken"];
            if (string.IsNullOrEmpty(token))
            {
                ViewData["ErrorMessage"] = "You are not logged in yet.";
                return RedirectToAction("SignIn", "Account");
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

                var appointments = _context.Appointments
                    .Where(a => a.UserId.ToString() == userId)
                    .ToList();

                return View(appointments);
            }
            catch (Exception ex)
            {
                // Log exception (ex) if needed
                ViewData["ErrorMessage"] = "Invalid or expired token. Please login again.";
                return RedirectToAction("SignIn", "Account");
            }
        }

        // Get: /Appointment/AddAppointment
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Appointment = this._context.Appointments.ToList();
            return View();
        }
        // POST: /Appointment/AddAppointment
        [HttpPost]
        public IActionResult Create(string Title, string Description, DateTime AppointmentDate)
        {
            // Ensure that the User is Logged In
            var token = Request.Cookies["authToken"];
            if (string.IsNullOrEmpty(token))
            {
                ViewData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("SignIn", "Account");
            }

            // Validate and decode the token
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]); // Ensure the key matches the one used for signing

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Extract user information from token
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

                if (AppointmentDate < DateTime.Now)
                {
                    ViewData["ErrorMessage"] = "Appointment date cannot be in the past.";
                    return View("Create");
                }

                var newAppointment = new Appointment
                {
                    UserId = int.Parse(userId),
                    Title = Title,
                    Description = Description,
                    AppointmentDate = AppointmentDate,
                    CreatedDate = DateTime.Now,
                };
                _context.Appointments.Add(newAppointment);
                _context.SaveChanges();
                return RedirectToAction("Appointment");
            }
            catch (Exception)
            {
                // Handle token validation errors
                ViewData["ErrorMessage"] = "Invalid or expired token. Please login again.";
                return RedirectToAction("SignIn", "Account");
            }
        }


        // GET: /Appointment/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewBag.Appointment = this._context.Appointments.ToList();
            return View("Create", appointment);
        }
        // POST: /Appointment/EditAppointment/{id}
        [HttpPost]
        public IActionResult Edit(int id, string Title, string Description, DateTime AppointmentDate)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            if (AppointmentDate < DateTime.Now)
            {
                ViewData["ErrorMessage"] = "Appointment date cannot be in the past.";
                return View("Create", appointment);
            }

            // Update appointment
            appointment.Title = Title;
            appointment.Description = Description;
            appointment.AppointmentDate = AppointmentDate;

            _context.Appointments.Update(appointment);
            int result = _context.SaveChanges();

            TempData["Message"] = "Appointment updated successfully.";

            if (result > 0)
            {
                var editNotification = _context.Notifications.FirstOrDefault(n => n.AppointmentId == appointment.AppointmentId);
                if (editNotification != null)
                {
                    editNotification.Notification_Data_Time = AppointmentDate.AddHours(-1); // تحديث الإشعار ليكون قبل الموعد بساعة
                    editNotification.IsSent = false;

                    _context.Notifications.Update(editNotification);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("appointment");
        }
        // GET: /Appointment/DeleteAppointment/{id}
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }
        // POST: /Appointment/DeleteAppointment/{id}
        [HttpPost, ActionName("Delete")]
        public IActionResult ConfirmDelete(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            TempData["Message"] = "Appointment deleted successfully.";
            return RedirectToAction("Appointment");
        }
    }
}
