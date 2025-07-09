using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.Utils;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Controllers.ProductManager
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        //===========================================================================================
        // Lấy giỏ hàng của người dùng
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null || currentUser.UserId == 0)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem giỏ hàng.";
                TempData["ReturnUrl"] = Url.Action("Cart", "Cart"); // Lưu URL để quay lại sau khi đăng nhập
                return RedirectToAction("Login", "Login");
            }

            var cartItems = await _cartService.GetCartByUserIdAsync(currentUser.UserId);
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
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null || currentUser.UserId == 0)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.";
                TempData["ReturnUrl"] = Url.Action("Cart", "Cart");
                return RedirectToAction("Login", "Login");
            }

            await _cartService.AddToCartAsync(currentUser.UserId, id, 1); // id là ProductId
            TempData["Success"] = "Đã thêm vào giỏ hàng.";
            return RedirectToAction("Cart", "Cart");
        }
        //===========================================================================================
        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null || currentUser.UserId == 0)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xóa sản phẩm khỏi giỏ hàng.";
                TempData["ReturnUrl"] = Url.Action("Cart", "Cart");
                return RedirectToAction("Login", "Login");
            }

            await _cartService.RemoveProductFromCart(productId,currentUser.UserId);
            TempData["Success"] = "Đã xóa khỏi giỏ hàng.";
            return RedirectToAction("Cart", "Cart");
        }
        //===========================================================================================
        // Cập nhật số lượng sản phẩm trong giỏ hàng
        //Tăng số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public async Task<IActionResult> IncreaseQuantityAjax(int productId)
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null) return Unauthorized();

            var updated = await _cartService.IncreaseQuantityAsync(productId, currentUser.UserId);

            return Json(new
            {
                newQuantity = updated.Quantity,
                itemTotal = (updated.Quantity * updated.Product.Price).ToString("N0"),
                cartTotal = (await _cartService.GetTotalAmountAsync(currentUser.UserId)).ToString("N0")
            });
        }
        // Giảm số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public async Task<IActionResult> DecreaseQuantityAjax(int productId)
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null) return Unauthorized();

            var updated = await _cartService.DecreaseQuantityAsync(productId, currentUser.UserId);

            return Json(new
            {
                newQuantity = updated.Quantity,
                itemTotal = (updated.Quantity * updated.Product.Price).ToString("N0"),
                cartTotal = (await _cartService.GetTotalAmountAsync(currentUser.UserId)).ToString("N0")
            });
        }

        //===========================================================================================

    }
}