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

      public async Task<IActionResult> Shop(int page = 1, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, string sort = null)
{
    int pageSize = 15;
    var products = await _productService.GetActiveProductsWithRatingAsync(page, pageSize, categoryId, minPrice, maxPrice, sort);

    // Lấy danh sách category cho dropdown
    var categories = await _productService.GetAllProductCategoriesAsync();
    ViewBag.Categories = categories;
    ViewBag.SelectedCategoryId = categoryId;
    ViewBag.MinPrice = minPrice;
    ViewBag.MaxPrice = maxPrice;
    ViewBag.Sort = sort;

    // Tính toán phân trang như cũ
    int totalProducts = await _productService.CountActiveProductsAsync(categoryId, minPrice, maxPrice);
    ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
    ViewBag.CurrentPage = page;

    return View(products);
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
        public async Task<IActionResult> Detail(int productID, int reviewPage = 1)
        {
            const int pageSize = 3;
            // Lấy chi tiết sản phẩm kèm đánh giá
            var product = await _productService.GetProductWithReviewsByIdAsync(productID);
            if (product == null) return NotFound();

            // Chuyển Product sang List<ReviewViewModel> (bao gồm cả reply)
            var allReviews = product.Reviews?.Select(r => new ReviewViewModel
            {
                UserId = r.User != null ? r.User.UserId : 0,
                ReviewerName = r.User != null
                    ? (!string.IsNullOrEmpty(r.User.FullName) ? r.User.FullName : r.User.Username)
                    : "Không xác định",
                UserAvatar = r.User != null && !string.IsNullOrEmpty(r.User.ProfilePictureUrl)
                    ? r.User.ProfilePictureUrl
                    : Url.Content("~/img/avatar.jfif"),
                Rating = r.Rating,
                Comment = r.Comment ?? string.Empty,
                CreatedAt = r.CreatedAt ?? DateTime.MinValue,
                Id = r.ReviewId,
                ParentReviewId = r.ParentReviewId
            }).ToList() ?? new List<ReviewViewModel>();

            // Build cây cha-con
            var reviewDict = allReviews.ToDictionary(r => r.Id);
            foreach (var review in allReviews)
            {
                if (review.ParentReviewId.HasValue && reviewDict.ContainsKey(review.ParentReviewId.Value))
                {
                    reviewDict[review.ParentReviewId.Value].Replies.Add(review);
                }
            }
            var parentReviews = allReviews.Where(r => r.ParentReviewId == null).OrderByDescending(r => r.CreatedAt).ToList();
            var reviewCount = parentReviews.Count;
            var averageRating = reviewCount > 0 ? (int)Math.Round(parentReviews.Average(r => r.Rating)) : 0;
            var productWithRating = new ProductWithRatingViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                CategoryName = product.ProductCategory?.Name,
                AverageRating = averageRating,
                ReviewCount = reviewCount,
                Reviews = parentReviews.Skip((reviewPage - 1) * pageSize).Take(pageSize).ToList()
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
                Reviews = new List<ReviewViewModel>() // Không cần nested cho suggested
            }).ToList();

            var viewModel = new ProductDetailViewModel
            {
                ProductWithRating = productWithRating,
                SuggestedProducts = suggestedProducts
            };

            ViewBag.CurrentUserId = GetCurrentUserId();
            ViewBag.ReviewPage = reviewPage;
            ViewBag.TotalReviewPages = (int)Math.Ceiling((double)reviewCount / pageSize);
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewsPage(int productId, int page = 1)
        {
            const int pageSize = 3;
            var product = await _productService.GetProductWithReviewsByIdAsync(productId);
            if (product == null) return NotFound();
            var allReviews = product.Reviews?.Select(r => new ReviewViewModel
            {
                UserId = r.User != null ? r.User.UserId : 0,
                ReviewerName = r.User != null
                    ? (!string.IsNullOrEmpty(r.User.FullName) ? r.User.FullName : r.User.Username)
                    : "Không xác định",
                UserAvatar = r.User != null && !string.IsNullOrEmpty(r.User.ProfilePictureUrl)
                    ? r.User.ProfilePictureUrl
                    : Url.Content("~/img/avatar.jfif"),
                Rating = r.Rating,
                Comment = r.Comment ?? string.Empty,
                CreatedAt = r.CreatedAt ?? DateTime.MinValue,
                Id = r.ReviewId,
                ParentReviewId = r.ParentReviewId
            }).ToList() ?? new List<ReviewViewModel>();
            var reviewDict = allReviews.ToDictionary(r => r.Id);
            foreach (var review in allReviews)
            {
                if (review.ParentReviewId.HasValue && reviewDict.ContainsKey(review.ParentReviewId.Value))
                {
                    reviewDict[review.ParentReviewId.Value].Replies.Add(review);
                }
            }
            var parentReviews = allReviews.Where(r => r.ParentReviewId == null).OrderByDescending(r => r.CreatedAt).ToList();
            int reviewCount = parentReviews.Count;
            int totalPages = (int)Math.Ceiling((double)reviewCount / pageSize);
            var pageReviews = parentReviews.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.ReviewPage = page;
            ViewBag.TotalReviewPages = totalPages;
            ViewBag.ProductId = productId;
            return PartialView("_ProductReviewsPartial", pageReviews);
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
            return HttpContext.Session.GetInt32("CurrentUserId") ?? 0;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Rating phải từ 1 đến 5." });
                }
                return BadRequest("Rating phải từ 1 đến 5.");
            }
            int userId = GetCurrentUserId();
            if (userId == 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để gửi đánh giá." });
                }
                return Unauthorized();
            }
            await _productService.AddProductReviewAsync(userId, productId, rating, comment, false);
            var product = await _productService.GetProductWithReviewsByIdAsync(productId);
            var allReviews = product.Reviews?.Select(r => new ReviewViewModel
            {
                ReviewerName = r.User != null
                    ? (!string.IsNullOrEmpty(r.User.FullName) ? r.User.FullName : r.User.Username)
                    : "Không xác định",
                UserAvatar = r.User != null && !string.IsNullOrEmpty(r.User.ProfilePictureUrl)
                    ? r.User.ProfilePictureUrl
                    : Url.Content("~/img/avatar.jfif"),
                Rating = r.Rating,
                Comment = r.Comment ?? string.Empty,
                CreatedAt = r.CreatedAt ?? DateTime.MinValue,
                Id = r.ReviewId,
                ParentReviewId = r.ParentReviewId
            }).ToList() ?? new List<ReviewViewModel>();
            // Build cây cha-con
            var reviewDict = allReviews.ToDictionary(r => r.Id);
            foreach (var review in allReviews)
            {
                if (review.ParentReviewId.HasValue && reviewDict.ContainsKey(review.ParentReviewId.Value))
                {
                    reviewDict[review.ParentReviewId.Value].Replies.Add(review);
                }
            }
            var parentReviews = allReviews.Where(r => r.ParentReviewId == null).OrderByDescending(r => r.CreatedAt).ToList();
            int reviewCount = parentReviews.Count;
            int totalPages = (int)Math.Ceiling((double)reviewCount / 3);
            ViewBag.ReviewPage = 1;
            ViewBag.TotalReviewPages = totalPages;
            ViewBag.ProductId = productId;
            return PartialView("_ProductReviewsPartial", parentReviews.Take(3).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddReply(int parentReviewId, string content)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }
            try
            {
                await _productService.AddReplyToReviewAsync(parentReviewId, userId, content);
            }
            catch (Exception ex)
            {
                // Nếu là AJAX, trả về JSON báo lỗi
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }
                return BadRequest(ex.Message);
            }
            // Lấy lại reply vừa tạo (chỉ lấy 1 reply mới nhất của user cho parentReviewId, không lọc theo content để tránh lặp)
            var reply = await _productService.GetLastReplyOfUserForParentAsync(parentReviewId, userId);
            if (reply == null)
            {
                return StatusCode(500, "Không tìm thấy phản hồi vừa tạo.");
            }
            var replyVM = new pet_spa_system1.ViewModel.ReviewViewModel
            {
                ReviewerName = reply.User != null
                    ? (!string.IsNullOrEmpty(reply.User.FullName) ? reply.User.FullName : reply.User.Username)
                    : "Không xác định",
                UserAvatar = reply.User != null && !string.IsNullOrEmpty(reply.User.ProfilePictureUrl)
                    ? reply.User.ProfilePictureUrl
                    : Url.Content("~/img/avatar.jfif"),
                Rating = reply.Rating,
                Comment = reply.Comment ?? string.Empty,
                CreatedAt = reply.CreatedAt ?? DateTime.MinValue,
                Id = reply.ReviewId,
                ParentReviewId = reply.ParentReviewId,
                UserId = reply.UserId
            };
            return PartialView("_ReviewRepliesPartial", replyVM);
        }
    }


    }
   