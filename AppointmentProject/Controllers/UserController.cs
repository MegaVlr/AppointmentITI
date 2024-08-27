using AppointmentProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using AppointmentProject.Models;
using System.IdentityModel.Tokens.Jwt;

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

        public User GetUserFromCookie()
        {
            if (Request.Cookies.TryGetValue("authToken", out var token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
                if (userIdClaim != null)
                {
                    // Convert userId from string to int
                    if (int.TryParse(userIdClaim.Value, out var userId))
                    {
                        return _context.Users.Find(userId);
                    }
                }
            }

            return null;
        }
        [HttpGet]
        // Action to display user information
        public IActionResult User()
        {
            var user = GetUserFromCookie();
            if (user != null)
            {
                ViewBag.User = user;
                return View();
            }

            return RedirectToAction("SignIn");
        }

        // GET: /User/EditUser/{id}
        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.User = user; 
            return View("EditUser", user); 
        }

        // POST: /User/EditAppointment/{id}
        [HttpPost]
        // POST: /User/EditUser/{id}
        [HttpPost]
        public IActionResult EditUser(int id, string Name, string Email, string PhoneNumber)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id); // تغيير Appointments إلى Users
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(PhoneNumber))
            {
                ViewBag.Message = "All fields are required.";
                return View(user);
            }

            // Check if email already exists (excluding the current user)
            var existingUser = _context.Users
                .SingleOrDefault(u => u.Email == Email && u.UserId != id);
            if (existingUser != null)
            {
                ViewBag.Message = "Email already exists.";
                return View(user);
            }

            // Check if phone number already exists (excluding the current user)
            var existingPhoneNumber = _context.Users
                .SingleOrDefault(p => p.phoneNumber == PhoneNumber && p.UserId != id);
            if (existingPhoneNumber != null)
            {
                ViewBag.Message = "Phone number already exists.";
                return View(user);
            }

            // Update user details
            user.Name = Name;
            user.Email = Email;
            user.phoneNumber = PhoneNumber;

            _context.Users.Update(user);
            int result = _context.SaveChanges();

            TempData["Message"] = "User updated successfully.";

            if (result > 0)
            {
                // Optional: Add additional logic if needed
            }

            return RedirectToAction("User");
        }

        // GET: /User/ChangePassword/{id}
        [HttpGet]
        public IActionResult ChangePassword() {
            
            
            return View(); }
        // POST: /User/ChangePassword/{id}
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            
            var user = GetUserFromCookie();

            if (user == null)
            {
                return RedirectToAction("SignIn"); // أو أي مسار مناسب إذا لم يكن المستخدم مسجلاً دخول
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

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Password changed successfully.";
            return RedirectToAction("User");
        }

        // GET: /User/DeleteUser/{id}
        [HttpGet]
        public IActionResult DeleteUser()
        {
            return View();
        }
        // POST: /User/DeleteUser/{id}
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            // ابحث عن المستخدم في قاعدة البيانات باستخدام المعرف (id)
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                // إذا كان المستخدم غير موجود، عرض رسالة خطأ
                ViewData["ErrorMessage"] = "User not found.";
                return View();
            }

            // احذف المستخدم من قاعدة البيانات
            _context.Users.Remove(user);
            _context.SaveChanges();

            // عرض رسالة تأكيد بأن الحذف تم بنجاح
            ViewData["Message"] = "User deleted successfully.";

            // إعادة توجيه إلى قائمة المستخدمين أو عرض صفحة الحذف مع الرسائل
            return RedirectToAction("User", "User");
        }

    }
}

