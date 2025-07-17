using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Services;

namespace pet_spa_system1.Controllers.ProductManager
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult AllOrder()
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId"); // Lấy userId từ session hoặc context;
            var orders = _orderService.GetOrdersByUserId(userId);
            return View(orders);
        }

        public IActionResult OrderStatusDetail(int orderId)
        {
            var order = _orderService.GetOrderDetail(orderId);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}
