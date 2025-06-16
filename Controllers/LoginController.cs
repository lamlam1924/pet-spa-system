using Microsoft.AspNetCore.Mvc;

namespace pet_spa_system1.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
