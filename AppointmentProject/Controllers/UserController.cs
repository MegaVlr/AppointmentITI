using AppointmentProject.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AppointmentProject.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AppointmentProject.Controllers
{
    public class UserController : Controller
    {
        private readonly AppointmentDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(AppointmentDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        private bool IsUserAuthenticated()
        {
            var token = Request.Cookies["authToken"];
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            return true;
        }
        private User GetUserFromCookie()
        {
            if (Request.Cookies.TryGetValue("authToken", out var token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
                if (userIdClaim != null)
                {
                    if (int.TryParse(userIdClaim.Value, out var userId))
                    {
                        return _context.Users.Find(userId);
                    }
                }
            }
            return null;
        }
        [HttpGet]
        public IActionResult User()
        {
            var user = GetUserFromCookie();
            if (user != null)
            {
                ViewBag.User = user;
                return View();
            }

            return RedirectToAction("User");
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            IsUserAuthenticated();

            if (IsUserAuthenticated() == false)
            {
                return RedirectToAction("SignIn", "Account");
            }

            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.User = user;
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(int id, string Name, string Email, string PhoneNumber)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(PhoneNumber))
            {
                ViewBag.Message = "All fields are required.";
                return View(user);
            }

            var existingUser = _context.Users
                .SingleOrDefault(u => u.Email == Email && u.UserId != id);
            if (existingUser != null)
            {
                ViewBag.Message = "Email already exists.";
                return View(user);
            }

            var existingPhoneNumber = _context.Users
                .SingleOrDefault(p => p.phoneNumber == PhoneNumber && p.UserId != id);
            if (existingPhoneNumber != null)
            {
                ViewBag.Message = "Phone number already exists.";
                return View(user);
            }

            user.Name = Name;
            user.Email = Email;
            user.phoneNumber = PhoneNumber;

            _context.Users.Update(user);
            int result = _context.SaveChanges();
            TempData["Message"] = "User updated successfully.";
            return RedirectToAction("User");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            IsUserAuthenticated();

            if (IsUserAuthenticated() == false)
            {
                return RedirectToAction("SignIn", "Account");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            var user = GetUserFromCookie();

            if (user == null)
            {
                return RedirectToAction("SignIn");
            }

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            {
                ViewData["ErrorMessage"] = "Old password is incorrect.";
                return View();
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                ViewData["ErrorMessage"] = "New password must be at least 6 characters long.";
                return View();
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            Response.Cookies.Delete("authToken");
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Password changed successfully.";
            return RedirectToAction("SignIn", "ACcount");
        }

        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            IsUserAuthenticated();

            if (IsUserAuthenticated() == false)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);


            if (user == null)
            {
                ViewData["ErrorMessage"] = "User not found.";
                return RedirectToAction("User");
            }

            return View();
        }

        [HttpPost]
        public IActionResult DeleteUser(int id, string Name)
        {
            IsUserAuthenticated();

            if (IsUserAuthenticated() == false)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                ViewData["ErrorMessage"] = "User not found.";
                return View();
            }

            _context.Users.Remove(user);
            var result = _context.SaveChanges();


            Response.Cookies.Delete("authToken");


            ViewData["ErrorMessage"] = "No changes were saved. Please try again.";

            TempData["Message"] = "Changes saved successfully.";
            return RedirectToAction("index", "Home");
        }
    }
}
