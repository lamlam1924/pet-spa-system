using Microsoft.AspNetCore.Mvc;

namespace pet_spa_system1.Controllers.Account1
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
