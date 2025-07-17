using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.Utils;
using pet_spa_system1.ViewModel;
using System.Threading.Tasks;

namespace pet_spa_system1.Controllers.ProductManager
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
        }

        //===========================================================================================
        // Lấy giỏ hàng của người dùng
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem giỏ hàng.";
                TempData["ReturnUrl"] = Url.Action("Cart", "Cart");
                return RedirectToAction("Login", "Login");
            }

            var cartItems = await _cartService.GetCartByUserIdAsync(userId.Value);
            var viewModel = new CartViewModel
            {
                Items = cartItems
            };
            return View(viewModel);
        }

        //===========================================================================================
        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.";
                TempData["ReturnUrl"] = Url.Action("Cart", "Cart");
                return RedirectToAction("Login", "Login");
            }

            try
            {
                await _cartService.AddToCartAsync(userId.Value, id, 1); // id là ProductId
                TempData["Success"] = "Đã thêm vào giỏ hàng.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi thêm vào giỏ hàng: {ex.Message}";
            }
            return RedirectToAction("Cart", "Cart");
        }

        // Thêm sản phẩm vào giỏ hàng với hiệu ứng AJAX
        [HttpPost]
        public async Task<IActionResult> AddToCartAjax(int id)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng." });
            }

            try
            {
                await _cartService.AddToCartAsync(userId.Value, id, 1);
                var cartItems = await _cartService.GetCartByUserIdAsync(userId.Value);
                int totalQuantity = cartItems.Sum(c => c.Quantity);
                decimal totalAmount = await _cartService.GetTotalAmountAsync(userId.Value);

                return Json(new { success = true, totalQuantity, totalAmount = totalAmount.ToString("N0") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        //===========================================================================================
        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xóa sản phẩm khỏi giỏ hàng.";
                TempData["ReturnUrl"] = Url.Action("Cart", "Cart");
                return RedirectToAction("Login", "Login");
            }

            try
            {
                await _cartService.RemoveProductFromCart(productId, userId.Value);
                TempData["Success"] = "Đã xóa khỏi giỏ hàng.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa khỏi giỏ hàng: {ex.Message}";
            }
            return RedirectToAction("Cart", "Cart");
        }

        //===========================================================================================
        // Tăng số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public async Task<IActionResult> IncreaseQuantityAjax(int productId)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null) return Unauthorized();

            try
            {
                var updated = await _cartService.IncreaseQuantityAsync(productId, userId.Value);
                decimal cartTotal = await _cartService.GetTotalAmountAsync(userId.Value);

                return Json(new
                {
                    success = true,
                    newQuantity = updated.Quantity,
                    itemTotal = (updated.Quantity * updated.Product.Price).ToString("N0"),
                    cartTotal = cartTotal.ToString("N0")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // Giảm số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public async Task<IActionResult> DecreaseQuantityAjax(int productId)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null) return Unauthorized();

            try
            {
                var updated = await _cartService.DecreaseQuantityAsync(productId, userId.Value);
                decimal cartTotal = await _cartService.GetTotalAmountAsync(userId.Value);

                return Json(new
                {
                    success = true,
                    newQuantity = updated.Quantity,
                    itemTotal = (updated.Quantity * updated.Product.Price).ToString("N0"),
                    cartTotal = cartTotal.ToString("N0")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}