using Microsoft.AspNetCore.Mvc;

namespace pet_spa_system1.Controllers.ProductManager
{
    public class OrderController : Controller
    {
        public IActionResult OrderStatusDetail()
        {
            return View();
        }

        public IActionResult AllOrder()
        {
            return View(); // Trả về view Error.cshtml trong thư mục Views/Order

        }
    }
}
