using Microsoft.AspNetCore.Mvc;

namespace pet_spa_system1.Controllers
{
    public class ServiceController : Controller
    {
        public IActionResult Booking()
        {
            return View();
        }
        public IActionResult ServicePet()
        {
            return View();
        }
    }
}
