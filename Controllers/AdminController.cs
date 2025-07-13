using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModels;
using pet_spa_system1.ViewModel;
using pet_spa_system1.Utils;

namespace pet_spa_system1.Controllers
{
    public class AdminController : Controller
    {
        private readonly PetDataShopContext _context;
        private readonly IProductService _productService;
        private readonly IServiceService _serviceService;
        private readonly IBlogService _blogService;

        public AdminController(PetDataShopContext context, IProductService productService, IServiceService serviceService, IBlogService blogService)
        {
            _context = context;
            _productService = productService;
            _serviceService = serviceService;
            _blogService = blogService;
        }
        //=======================================================================================================================
        // SERVICE
        public IActionResult ManageService(int? categoryId, string search)
        {
            var model = _serviceService.GetAllService();
            if (categoryId.HasValue)
            {
                model.Services = model.Services.Where(s => s.CategoryId == categoryId.Value).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                model.Services = model.Services.Where(s => s.Name.Contains(search)).ToList();
            }
            model.SelectedCategoryId = categoryId;
            return View(model);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Appointments()
        {
            return View();
        }

        public IActionResult Payment()
        {
            return View();
        }

        public IActionResult Pets_List()
        {
            return View();
        }

        public IActionResult List_Customer()
        {
            return View();
        }

        

        

        //=======================================================================================================================
        //--PRODUCT--
        public async Task<IActionResult> Product_Detail(int productID)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productID);

            if (product == null)
            {
                return NotFound();
            }

            // Truyền dữ liệu sản phẩm vào View
            return View(product);
        }

        public async Task<IActionResult> Products_List(int page = 1)
        {

            const int pageSize = 15; // Số sản phẩm trên mỗi trang
            var totalProducts = await _context.Products.CountAsync();
            var products = await _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.Category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;

            return View(products);
        }
        
        

        public async Task<IActionResult> Add_New_Product()
        {
            var categories = await _productService.GetAllProductCategoriesAsync();
            var viewModel = new ProductViewModel
            {
                Product = new Product(),
                SuggestedProducts = new List<Product>(), // không dùng trong add
                Categories = categories
            };
            return View(viewModel);
        }

         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add_New_Product(ProductViewModel viewModel, IFormFile Image)
        {
            ModelState.Remove("Product.Category");
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    if (state.Value?.Errors.Count > 0)
                    {
                        Console.WriteLine($"❌ ERROR AT: {state.Key}");
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"   ➤ {error.ErrorMessage}");
                        }
                    }
                }
                // Gửi lại categories nếu có lỗi
                viewModel.Categories = await _productService.GetAllProductCategoriesAsync();
                return View(viewModel);
            }

            var product = viewModel.Product;

            // Upload ảnh
            if (Image != null && Image.Length > 0)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgProducts");
                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(imagePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                product.ImageUrl = "/imgProducts/" + fileName;
            }

            product.CreatedAt = DateTime.Now;
            if (product.IsActive == null)
            {
                product.IsActive = true;
            }

            try
            {
                // thêm hoặc cập nhật
                await _productService.CreateProductAsync(product);
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Add_New_Product");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Add_New_Product");
            }

        }

        //Edit Products
        public async Task<IActionResult> Edit_Products(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _productService.GetAllProductCategoriesAsync();
            var suggestedProducts = await _productService.GetAllProductsAsync(1, 4); // Lấy 4 sản phẩm gợi ý
            var viewModel = new ProductViewModel
            {
                Product = product,
                SuggestedProducts = suggestedProducts.Where(p => p.ProductId != id).ToList(), // Loại bỏ sản phẩm hiện tại khỏi gợi ý
                Categories = categories
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit_Products(ProductViewModel viewModel, IFormFile? Image)
        {
            var product = await _productService.GetProductByIdAsync(viewModel.Product.ProductId);
            if (product == null) return NotFound();

            // Cập nhật các trường từ form
            product.Name = viewModel.Product.Name;
            product.Description = viewModel.Product.Description;
            product.Price = viewModel.Product.Price;
            product.Stock = viewModel.Product.Stock;
            product.CategoryId = viewModel.Product.CategoryId;
            product.IsActive = viewModel.Product.IsActive;

            // Nếu có ảnh mới
            if (Image != null && Image.Length > 0)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgProducts");
                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(imagePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                product.ImageUrl = "/imgProducts/" + fileName;
            }

            // Cập nhật vào CSDL
            await _productService.UpdateProductAsync(product);
            try
            {
                // thêm hoặc cập nhật
                await _productService.UpdateProductAsync(product);
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Edit_Products", new { id = product.ProductId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Edit_Products", new { id = product.ProductId });
            }


           
        }


        //=======================================================================================================================

        // Disable Product

        [HttpPost]
        public async Task<IActionResult> DisableProduct(int id)
        {
            await _productService.DisableProductAsync(id);
            TempData["SuccessMessage"] = "Đã ngừng kích hoạt sản phẩm.";
            return RedirectToAction("Products_List");
        }

        //=======================================================================================================================

        // Delete Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("Products_List");
        }

        //=======================================================================================================================


        public IActionResult Refund()
        {
            return View();
        }

        public IActionResult Staff()
        {
            return View();
        }

        public IActionResult StaffSchedule()
        {
            return View("StaffSchedule");
        }

        //=======================================================================================================================
        // BLOG MANAGEMENT
        public async Task<IActionResult> ManageBlog(string status = "All", string? search = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null || (currentUser.RoleId != 1 && currentUser.RoleId != 3)) // Admin or Staff
            {
                return RedirectToAction("Login", "Login");
            }

            var model = await _blogService.GetAdminDashboardAsync();

            model.AllBlogs = await _blogService.GetAllBlogsForAdminAsync();

            if (status != "All")
            {
                model.AllBlogs = model.AllBlogs.Where(b => b.Status == status).ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                model.AllBlogs = model.AllBlogs.Where(b =>
                    b.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.ShortContent.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.AuthorName.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (fromDate.HasValue)
            {
                model.AllBlogs = model.AllBlogs.Where(b => b.CreatedAt >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                model.AllBlogs = model.AllBlogs.Where(b => b.CreatedAt <= toDate.Value.AddDays(1)).ToList();
            }

            model.StatusFilter = status;
            model.SearchQuery = search;
            model.FromDate = fromDate;
            model.ToDate = toDate;

            if (!string.IsNullOrEmpty(search))
            {
                model.AllBlogs = model.AllBlogs.Where(b =>
                    b.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.AuthorName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.StatusFilter = status;
            ViewBag.SearchQuery = search;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBlog(int blogId)
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null || (currentUser.RoleId != 1 && currentUser.RoleId != 3)) 
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            try
            {
                var success = await _blogService.ApproveBlogAsync(blogId, currentUser.UserId);
                if (success)
                {
                    return Json(new { success = true, message = "Blog đã được duyệt thành công." });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể duyệt blog này." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectBlog(int blogId, string? reason = null)
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null || (currentUser.RoleId != 1 && currentUser.RoleId != 3)) 
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            try
            {
                var success = await _blogService.RejectBlogAsync(blogId, currentUser.UserId, reason);
                if (success)
                {
                    return Json(new { success = true, message = "Blog đã bị từ chối." });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể từ chối blog này." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishBlog(int blogId)
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null || (currentUser.RoleId != 1 && currentUser.RoleId != 3)) 
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            try
            {
                var success = await _blogService.PublishBlogAsync(blogId, currentUser.UserId);
                if (success)
                {
                    return Json(new { success = true, message = "Blog đã được xuất bản." });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xuất bản blog này." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlogAdmin(int blogId)
        {
            var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null || (currentUser.RoleId != 1 && currentUser.RoleId != 3)) 
            {
                return Json(new { success = false, message = "Không có quyền thực hiện." });
            }

            try
            {
                var success = await _blogService.DeleteBlogAsync(blogId, currentUser.UserId);
                if (success)
                {
                    return Json(new { success = true, message = "Blog đã được xóa." });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa blog này." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }


    }
}
