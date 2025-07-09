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

            var products = await _productService.GetActiveProductsWithRatingAsync(page, pageSize);
            var total = await _productService.CountActiveProductsAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
            ViewBag.CurrentPage = page;

            return View(products); // truyền List<ProductWithRatingViewModel>
        }


        //=========================================================================================

        //public async Task<IActionResult> Product_List(int id)
        //{
        //    var product = await _productService.GetProductByIdAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewBag.Categories = await _productService.GetAllProductCategoriesAsync();
        //    return View(product);
        //}

        //=========================================================================================
        // Product detail with reviews and suggestions
        public async Task<IActionResult> Detail(int productID)
        {
            // Lấy chi tiết sản phẩm kèm đánh giá
            var product = await _productService.GetProductWithReviewsByIdAsync(productID);
            if (product == null) return NotFound();

            // Chuyển Product sang ProductWithRatingViewModel
            var productWithRating = new ProductWithRatingViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                CategoryName = product.ProductCategory?.Name,
                AverageRating = product.Reviews != null && product.Reviews.Count > 0
                    ? (int)Math.Round(product.Reviews.Average(r => r.Rating))
                    : 0,
                ReviewCount = product.Reviews?.Count ?? 0,
                Reviews = product.Reviews?.Select(r => new ReviewViewModel
                {
                    ReviewerName = r.User?.FullName ?? r.User?.Username ?? "Ẩn danh",
                    Rating = r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    CreatedAt = r.CreatedAt ?? DateTime.MinValue
                }).ToList() ?? new List<ReviewViewModel>()
            };

            // Lấy danh sách sản phẩm gợi ý (cùng loại, loại trừ sản phẩm hiện tại)
            var (_, suggestedProductsList) = await _productService.GetProductDetailWithSuggestionsAsync(productID);
            var suggestedProducts = suggestedProductsList.Select(s => new ProductWithRatingViewModel
            {
                ProductId = s.ProductId,
                Name = s.Name,
                ImageUrl = s.ImageUrl,
                Price = s.Price,
                CategoryName = s.ProductCategory?.Name,
                AverageRating = s.Reviews != null && s.Reviews.Count > 0
                    ? (int)Math.Round(s.Reviews.Average(r => r.Rating))
                    : 0,
                ReviewCount = s.Reviews?.Count ?? 0,
                Reviews = s.Reviews?.Select(r => new ReviewViewModel
                {
                    ReviewerName = r.User?.FullName ?? r.User?.Username ?? "Ẩn danh",
                    Rating = r.Rating,
                    Comment = r.Comment ?? string.Empty,
                    CreatedAt = r.CreatedAt ?? DateTime.MinValue
                }).ToList() ?? new List<ReviewViewModel>()
            }).ToList();

            var viewModel = new ProductDetailViewModel
            {
                ProductWithRating = productWithRating,
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
        //========================================================================================
        private int GetCurrentUserId()
        {
            return int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
        }
    }


    }
   