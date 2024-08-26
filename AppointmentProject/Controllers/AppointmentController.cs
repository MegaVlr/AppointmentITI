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

        // GET: /Appointment/ShowApp
        [HttpGet]
        public IActionResult ShowApp()
        {
            var appointments = _context.Appointments.ToList();
            return View(appointments);
        }

        // POST: /Appointment/AddAppointment
        [HttpPost]
        public IActionResult AddAppointment(string Title, string Description, DateTime AppointmentDate)
        {
            if (string.IsNullOrEmpty(Title))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            if (AppointmentDate < DateTime.Now)
            {
                ModelState.AddModelError("", "Appointment date cannot be in the past.");
                return View();
            }

            // Create new appointment
            var newAppointment = new Appointment
            {
                Title = Title,
                Description = Description,
                AppointmentDate = AppointmentDate,
                CreatedDate = DateTime.Now
            };

            _context.Appointments.Add(newAppointment);
            _context.SaveChanges();

            TempData["Message"] = "Appointment added successfully.";
            return RedirectToAction("ShowApp"); // Redirect to the list of appointments
        }

        // GET: /Appointment/EditAppointment/{id}
        [HttpGet]
        public IActionResult EditAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: /Appointment/EditAppointment/{id}
        [HttpPost]
        public IActionResult EditAppointment(int id, string Title, string Description, DateTime AppointmentDate)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Description))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View(appointment);
            }

            if (AppointmentDate < DateTime.Now)
            {
                ModelState.AddModelError("", "Appointment date cannot be in the past.");
                return View(appointment);
            }

            // Update appointment
            appointment.Title = Title;
            appointment.Description = Description;
            appointment.AppointmentDate = AppointmentDate;

            _context.Appointments.Update(appointment);
            _context.SaveChanges();

            TempData["Message"] = "Appointment updated successfully.";
            return RedirectToAction("ShowApp");
        }

        // GET: /Appointment/DeleteAppointment/{id}
        [HttpGet]
        public IActionResult DeleteAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: /Appointment/DeleteAppointment/{id}
        [HttpPost, ActionName("DeleteAppointment")]
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
            return RedirectToAction("ShowApp");
        }
    }
}
