using Microsoft.AspNetCore.Mvc;

namespace HealthCareApp.Controllers.Doctor
{
    public class DoctorAvailability : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
