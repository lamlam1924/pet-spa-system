using Microsoft.AspNetCore.Mvc;

namespace pet_spa_system1.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }
    }
}
