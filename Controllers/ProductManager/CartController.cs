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
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _productService = productService;
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

        [HttpGet]
        public async Task<IActionResult> CheckStock(int productId, int quantity)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            if (quantity > product.Stock)
            {
                return Json(new
                {
                    success = false,
                    message = $"Chỉ còn lại {product.Stock} sản phẩm trong kho.",
                    availableStock = product.Stock
                });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateCartRequest request)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
                return Unauthorized();

            if (request.Quantity < 1)
                return BadRequest(new { message = "Số lượng không hợp lệ." });

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product == null)
                return NotFound(new { message = "Sản phẩm không tồn tại." });

            if (request.Quantity > product.Stock)
                return BadRequest(new { message = $"Chỉ còn lại {product.Stock} sản phẩm trong kho." });

            // Lấy danh sách giỏ hàng của người dùng
            var cartItems = await _cartService.GetCartByUserIdAsync(userId.Value);
            if (cartItems == null || cartItems.Count == 0)
                return NotFound(new { message = "Giỏ hàng không tồn tại hoặc rỗng." });

            // Tìm item cần cập nhật trong giỏ hàng
            var cartItem = cartItems.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (cartItem == null)
                await _cartService.AddToCartAsync(userId.Value,product.ProductId, request.Quantity);

            // Gọi service cập nhật số lượng (theo CartId)
            await _cartService.UpdateQuantityAsync(cartItem.CartId, request.Quantity);

            return Ok(new { message = "Cập nhật số lượng thành công." });
        }
        [HttpGet]
        public async Task<IActionResult> CheckCartBeforeCheckout()
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập." });
            }

            var cartItems = await _cartService.GetCartByUserIdAsync(userId.Value);
            var invalidItems = cartItems
                .Where(c => c.Quantity > c.Product.Stock)
                .Select(c => new { c.Product.Name, Stock = c.Product.Stock, Quantity = c.Quantity })
                .ToList();

            if (invalidItems.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "Một số sản phẩm vượt quá tồn kho.",
                    details = invalidItems
                });
            }

            return Json(new { success = true });
        }




    }
}