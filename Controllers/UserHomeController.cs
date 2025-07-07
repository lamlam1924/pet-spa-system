using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Controllers
{
    public class UserHomeController : Controller
    {
        public IActionResult Index()
        {
            var userModel = new UserViewModel
            {
                UserName = "PhucLe",
                AvatarUrl = "/images/default-avatar.jpg",
                Email = "phucle@example.com",
                PhoneNumber = "0123456789"
            };

            return View(userModel); // truyền model vào view
        }
    }
}
