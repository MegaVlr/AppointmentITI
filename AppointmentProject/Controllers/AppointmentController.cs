using Microsoft.AspNetCore.Mvc;
namespace AppointmentProject.Controllers
{
    public class AppointmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Appointment()
        {
            return View();
        }
    }
}
