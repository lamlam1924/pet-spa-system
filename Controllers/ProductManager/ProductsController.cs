using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Controllers.ProductManager
{
    public class ProductsController : Controller
    {
       
            private readonly IProductService _productService;
            private readonly ICartService _cartService;

        public ProductsController(IProductService productService, ICartService cartService)
            {
                _productService = productService;
                _cartService = cartService;
        }

        public async Task<IActionResult> Shop(int page = 1)
        {
            const int pageSize = 15;

            var products = await _productService.GetActiveProductsAsync(page, pageSize);
            var totalActiveProducts = await _productService.CountActiveProductsAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalActiveProducts / pageSize);
            ViewBag.CurrentPage = page;

            return View(products);
        }

        


        public async Task<IActionResult> Detail(int productID)
        {
            var (product, suggestedProducts) = await _productService.GetProductDetailWithSuggestionsAsync(productID);
            if (product == null) return NotFound();

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                SuggestedProducts = suggestedProducts
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _productService.GetAllProductCategoriesAsync(); // Giả định phương thức này tồn tại
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi lấy danh sách danh mục: " + ex.Message);
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> Cart(int id)
        {
            var userId = GetCurrentUserId(); // Hàm tự viết để lấy user hiện tại
            await _cartService.AddToCartAsync(new Cart
            {
                UserId = userId,
                ProductId = id,
                Quantity = 1, // Hoặc lấy từ input nếu có
                AddedAt = DateTime.Now
            });
            return RedirectToAction("Cart");
        }

        private int GetCurrentUserId()
    {
        return int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
    }
        }
    }
   