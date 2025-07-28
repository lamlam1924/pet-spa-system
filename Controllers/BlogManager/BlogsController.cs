using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;
using pet_spa_system1.Utils;
using pet_spa_system1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace pet_spa_system1.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly IUserService _userService;

        public BlogsController(IBlogService blogService, IUserService userService)
        {
            _blogService = blogService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(int page = 1, string? category = null, string? search = null, string sortBy = "newest")
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            ViewBag.CurrentUser = userId.HasValue ? new User { UserId = userId.Value } : null;
            var model = await _blogService.GetBlogListAsync(page, 10, category, search, sortBy);
            return View(model);
        }

        public async Task<IActionResult> MyBlogs()
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            if (currentUserRoleId != 1 && currentUserRoleId != 3) // Chỉ Admin/Staff xem MyBlogs
            {
                TempData["ErrorMessage"] = "Bạn không có quyền xem danh sách bài viết của mình.";
                return RedirectToAction("Index");
            }

            var blogs = await _blogService.GetBlogsByUserAsync(userId.Value);
            ViewBag.CurrentUser = new User { UserId = userId.Value };
            return View(blogs);
        }

        public async Task<IActionResult> Blog(int page = 1, string? category = null, string? search = null, string sortBy = "newest")
        {
            return await Index(page, category, search, sortBy);
        }

        [Route("Blogs/Detail/{id:int}")]
        public async Task<IActionResult> Detail(int id, int? userId = null)
        {
            Console.WriteLine($"[BlogsController] Rendering Detail for BlogId: {id}, UserId from query: {userId}");
            var sessionUserId = HttpContext.Session.GetInt32("CurrentUserId");
            var effectiveUserId = userId ?? sessionUserId;
            Console.WriteLine($"[BlogsController] Effective UserId: {effectiveUserId}");
            var model = await _blogService.GetBlogDetailAsync(id, effectiveUserId);
            if (model == null) return NotFound();
            ViewBag.CurrentUser = effectiveUserId.HasValue ? new User { UserId = effectiveUserId.Value } : null;
            return View("Single_Blog", model);
        }

        public async Task<IActionResult> Create()
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            if (currentUserRoleId != 1 && currentUserRoleId != 3) // Chỉ Admin/Staff được tạo
            {
                TempData["ErrorMessage"] = "Bạn không có quyền tạo bài viết.";
                return RedirectToAction("Index");
            }

            var model = await _blogService.GetBlogCreateViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCreateViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            if (currentUserRoleId != 1 && currentUserRoleId != 3) // Chỉ Admin/Staff được tạo
            {
                TempData["ErrorMessage"] = "Bạn không có quyền tạo bài viết.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                model.AvailableCategories = await _blogService.GetCategoriesAsync();
                return View(model);
            }

            try
            {
                var blogId = await _blogService.CreateBlogAsync(model, userId.Value);

                if (currentUserRoleId == 1 || currentUserRoleId == 3) // Admin/Staff tự xuất bản
                {
                    await _blogService.PublishBlogAsync(blogId, userId.Value);
                    TempData["SuccessMessage"] = "Blog đã được tạo và xuất bản thành công.";
                }

                return RedirectToAction("Detail", new { id = blogId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo blog: " + ex.Message);
                model.AvailableCategories = await _blogService.GetCategoriesAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> RejectedBlogs()
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            if (currentUserRoleId != 1 && currentUserRoleId != 3) // Chỉ Admin/Staff xem rejected blogs
            {
                TempData["ErrorMessage"] = "Bạn không có quyền xem bài viết bị từ chối.";
                return RedirectToAction("Index");
            }

            var rejectedBlogs = await _blogService.GetRejectedBlogsByUserAsync(userId.Value);
            return View(rejectedBlogs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRejectedBlog(int blogId, BlogCreateViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            if (currentUserRoleId != 1 && currentUserRoleId != 3) // Chỉ Admin/Staff cập nhật
            {
                TempData["ErrorMessage"] = "Bạn không có quyền cập nhật bài viết.";
                return RedirectToAction("RejectedBlogs");
            }

            if (!ModelState.IsValid)
            {
                return View("RejectedBlogs", model);
            }

            try
            {
                var success = await _blogService.UpdateBlogAsync(blogId, model, userId.Value);
                if (success)
                {
                    if (currentUserRoleId == 1 || currentUserRoleId == 3) // Admin/Staff tự xuất bản
                    {
                        await _blogService.PublishBlogAsync(blogId, userId.Value);
                        TempData["SuccessMessage"] = "Bài viết đã được cập nhật và xuất bản.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Bài viết đã được cập nhật và gửi lại để duyệt.";
                    }
                    return RedirectToAction("RejectedBlogs");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật bài viết.";
                    return View("RejectedBlogs", model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return View("RejectedBlogs", model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            if (currentUserRoleId != 1 && currentUserRoleId != 3) // Chỉ Admin/Staff chỉnh sửa
            {
                TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa bài viết.";
                return RedirectToAction("Index");
            }

            var blog = await _blogService.GetBlogDetailAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            // Dùng session để xác định role thay vì gọi DbContext
            var userRole = GetUserRole(currentUserRoleId);
            if (!_blogService.CanUserEditBlog(id, userId.Value, userRole))
            {
                return Forbid();
            }

            var categories = await _blogService.GetCategoriesAsync();
            var availableCategories = categories.Select(c => new SelectListItem
            {
                Value = c,
                Text = c
            }).ToList();

            var model = new BlogCreateViewModel
            {
                Title = blog.Blog.Title,
                Content = blog.Blog.Content,
                Category = blog.Blog.Category,
                Status = blog.Blog.Status ?? "Draft",
                FeaturedImageUrl = blog.Blog.FeaturedImageUrl,
                AvailableCategories = categories
            };

            ViewBag.AvailableCategories = availableCategories;
            ViewBag.BlogId = id;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogCreateViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            if (currentUserRoleId != 1 && currentUserRoleId != 3) // Chỉ Admin/Staff chỉnh sửa
            {
                TempData["ErrorMessage"] = "Bạn không có quyền chỉnh sửa bài viết.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                var categories = await _blogService.GetCategoriesAsync();
                ViewBag.AvailableCategories = categories.Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                }).ToList();
                ViewBag.BlogId = id;
                return View(model);
            }

            try
            {
                var success = await _blogService.UpdateBlogAsync(id, model, userId.Value);
                if (!success)
                {
                    return NotFound();
                }

                if (currentUserRoleId == 1 || currentUserRoleId == 3) // Admin/Staff tự xuất bản
                {
                    await _blogService.PublishBlogAsync(id, userId.Value);
                    TempData["SuccessMessage"] = "Bài viết đã được cập nhật và xuất bản thành công.";
                }

                return RedirectToAction("Detail", new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật bài viết: " + ex.Message);
                var categories = await _blogService.GetCategoriesAsync();
                ViewBag.AvailableCategories = categories.Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c
                }).ToList();
                ViewBag.BlogId = id;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBlog(int blogId)
        {
            Console.WriteLine($"[BlogController] DeleteBlog POST called - BlogId: {blogId}, Request: {HttpContext.Request.Path}");

            var currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
            Console.WriteLine($"[BlogController] CurrentUserId: {currentUserId ?? -1}");

            if (!currentUserId.HasValue)
            {
                Console.WriteLine("[BlogController] User not logged in.");
                return Json(new { success = false, message = "Vui lòng đăng nhập để xóa bài viết." });
            }

            var currentUserRoleId = HttpContext.Session.GetInt32("CurrentUserRoleId") ?? -1;
            if (currentUserRoleId != 1 && currentUserRoleId != 3) // Chỉ Admin/Staff xóa
            {
                Console.WriteLine("[BlogController] Access denied: Insufficient permissions to delete blog.");
                return Json(new { success = false, message = "Bạn không có quyền xóa bài viết." });
            }

            try
            {
                Console.WriteLine("[BlogController] Attempting to delete blog...");
                var success = await _blogService.DeleteBlogAsync(blogId, currentUserId.Value);
                if (success)
                {
                    Console.WriteLine("[BlogController] Blog deleted successfully.");
                    return Json(new { success = true, message = "Bài viết đã được xóa thành công!" });
                }
                else
                {
                    Console.WriteLine("[BlogController] Failed to delete blog.");
                    return Json(new { success = false, message = "Không thể xóa bài viết này." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BlogController] Error deleting blog: {ex.Message} - StackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa bài viết: " + ex.Message });
            }
        }

      [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AddComment(int blogId, string content, int? parentCommentId = null)
{
    System.Diagnostics.Debug.WriteLine($"AddComment called: blogId={blogId}, content={content}, parentCommentId={parentCommentId}");

    var userId = HttpContext.Session.GetInt32("CurrentUserId") ?? RouteData.Values["userId"] as int?;
    if (!userId.HasValue)
    {
        System.Diagnostics.Debug.WriteLine("User not logged in");
        return Json(new { success = false, message = "Bạn cần đăng nhập để bình luận." });
    }

    if (string.IsNullOrWhiteSpace(content))
    {
        return Json(new { success = false, message = "Nội dung bình luận không được để trống." });
    }

    // Kiểm tra bài viết hợp lệ và đã được duyệt
    var blog = await _blogService.GetBlogByIdAsync(blogId);
    if (blog == null || blog.Status != "Published")
    {
        System.Diagnostics.Debug.WriteLine($"Blog {blogId} not found or not published, status: {blog?.Status}");
        return Json(new { success = false, message = "Bài viết chưa được duyệt, không thể bình luận." });
    }

    // Kiểm tra comment cha nếu có
    if (parentCommentId.HasValue)
    {
        var parentComment = await _blogService.GetCommentByIdAsync(parentCommentId.Value);
        if (parentComment == null || parentComment.BlogId != blogId)
        {
            System.Diagnostics.Debug.WriteLine($"Invalid parentCommentId: {parentCommentId.Value} not found or not in blog {blogId}");
            return Json(new { success = false, message = "Bình luận cha không hợp lệ." });
        }
    }

    try
    {
        var userInfo = await _userService.GetUserByIdAsync(userId.Value); // Lấy FullName, Username, RoleId
        var success = await _blogService.AddCommentAsync(blogId, content.Trim(), userId.Value, parentCommentId);
        System.Diagnostics.Debug.WriteLine($"AddCommentAsync result: {success}, parentCommentId used: {parentCommentId}");

        if (success)
        {
            var userRole = GetUserRole(userInfo?.RoleId ?? 0);
            var createdAt = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            return Json(new
            {
                success = true,
                message = parentCommentId.HasValue ? "Trả lời đã được gửi thành công!" : "Bình luận đã được gửi thành công!",
                userName = userInfo?.FullName ?? userInfo?.Username,
                userRole = userRole,
                createdAt = createdAt,
                content = content.Trim(),
                parentCommentId = parentCommentId,
                status = "Approved"
            });
        }
        else
        {
            return Json(new { success = false, message = "Có lỗi xảy ra khi gửi bình luận." });
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Exception in AddComment: {ex.Message}");
        return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
    }
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike(int blogId)
        {
            var userId = HttpContext.Session.GetInt32("CurrentUserId") ?? RouteData.Values["userId"] as int?;
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thích bài viết." });
            }

            var blog = await _blogService.GetBlogByIdAsync(blogId); // ✅ dùng service thay vì context
            if (blog == null || blog.Status != "Published")
            {
                System.Diagnostics.Debug.WriteLine($"Blog {blogId} not found or not published, status: {blog?.Status}");
                return Json(new { success = false, message = "Bài viết chưa được duyệt, không thể thích." });
            }

            try
            {
                var isLiked = await _blogService.ToggleLikeAsync(blogId, userId.Value);
                var likeCount = await _blogService.GetLikeCountAsync(blogId);

                return Json(new
                {
                    success = true,
                    isLiked = isLiked,
                    likeCount = likeCount,
                    message = isLiked ? "Đã thích bài viết" : "Đã bỏ thích bài viết"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentBlogs(int count = 5)
        {
            var blogs = await _blogService.GetRecentBlogsAsync(count);
            return Json(blogs);
        }

        [HttpGet]
        public async Task<IActionResult> GetPopularBlogs(int count = 5)
        {
            var blogs = await _blogService.GetPopularBlogsAsync(count);
            return Json(blogs);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _blogService.GetCategoriesAsync();
            return Json(categories);
        }

        private string GetUserRole(int roleId)
        {
            return roleId switch
            {
                1 => "Admin",
                2 => "Customer",
                3 => "Staff",
                _ => "Customer"
            };
        }
    }
}