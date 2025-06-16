using Microsoft.AspNetCore.Mvc;

namespace pet_spa_system1.Controllers
{
    public class BlogsController : Controller
    {
        public IActionResult Blog()
        {
            return View();
        }
        public IActionResult Single_Blog()
        {
            return View();
        }
    }
}
