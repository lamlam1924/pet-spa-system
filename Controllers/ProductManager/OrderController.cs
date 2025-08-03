using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Services;

namespace pet_spa_system1.Controllers.ProductManager
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        public IActionResult AllOrder(int? statusId, int? orderId, int page = 1, int pageSize = 5)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");

            int totalOrders;
            var orders = _orderService.GetOrdersByUserIdPaged(userId, page, pageSize, out totalOrders, statusId, orderId);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.StatusId = statusId;
            ViewBag.OrderIdSearch = orderId?.ToString();

            return View(orders);
        }



        public IActionResult OrderStatusDetail(int orderId)
        {
            var order = _orderService.GetOrderDetail(orderId);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public IActionResult CancelOrder(int orderId)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            var result = _orderService.CancelOrder(orderId, userId);
            if (!result)
            {
                TempData["Error"] = "Không thể hủy đơn hàng này!";
            }
            else
            {
                TempData["Success"] = "Đã hủy đơn hàng thành công!";
            }
            return RedirectToAction("AllOrder");
        }

        [HttpPost]
        public IActionResult BuyAgain(int orderId)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            var order = _orderService.GetOrderDetail(orderId);
            if (order == null || userId == null) return NotFound();

            // Thêm lại sản phẩm vào giỏ hàng
            foreach (var item in order.Items)
            {
                _cartService.AddToCartAsync(userId.Value, item.ProductId, item.Quantity).Wait();
            }
            return RedirectToAction("Cart", "Cart");
        }
    }
}
