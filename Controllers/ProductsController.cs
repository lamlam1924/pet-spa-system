using Microsoft.AspNetCore.Mvc;

namespace pet_spa_system1.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Shop()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Detail()
        {
            return View();
        }
        public IActionResult Checkout()
        {
            return View();
        }
    }
}
