using AppointmentProject.Data;
using AppointmentProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AppointmentProject.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppointmentDbContext _context;

        public AppointmentController(AppointmentDbContext context)
        {
            _context = context;
        }
        // GET: /Appointment/Appointment
        [HttpGet]
        public IActionResult Appointment()
            
        {
            List <Appointment> appointments = _context.Appointments.ToList(); // Fetch all appointments
            return View(appointments);
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
            if (AppointmentDate < DateTime.Now)
            {
                ViewData["ErrorMessage"] = "Appointment date cannot be in the past.";
                return View("Create");
            }

            var newAppointment = new Appointment
            {
                UserId = 1, 
                Title = Title,
                Description = Description,
                AppointmentDate = AppointmentDate,
                CreatedDate = DateTime.Now,
            };
            _context.Appointments.Add(newAppointment);
            int result = _context.SaveChanges();


            if (result > 0)
            {
                var newNotification = new Notification
                {
                    AppointmentId = newAppointment.AppointmentId, // ربط الإشعار بالموعد
                    Notification_Data_Time = AppointmentDate.AddHours(-1),
                    IsSent = false
                };

                _context.Notifications.Add(newNotification);
                _context.SaveChanges();
            }
            return RedirectToAction("appointment");
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
