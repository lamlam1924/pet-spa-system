using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModels;
using System.Diagnostics;

namespace pet_spa_system1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotificationService _notificationService;

        public HomeController(ILogger<HomeController> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (!userId.HasValue)
            {
                return Json(new { unreadCount = 0 });
            }

            var notifications = await _notificationService.GetByUserIdAsync(userId.Value);
            var unreadCount = notifications.Count(n => !n.IsRead);

            return Json(new { unreadCount });
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (!userId.HasValue)
            {
                return Json(new { notifications = new List<object>() });
            }

            var notifications = await _notificationService.GetByUserIdAsync(userId.Value);

            var notificationData = notifications.Select(n => new
            {
                title = n.Title,
                message = n.Message,
                createdAt = n.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                isRead = n.IsRead
            }).ToList();

            return Json(new { notifications = notificationData });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("CurrentUserId");
                if (!userId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in" });
                }

                await _notificationService.MarkAllAsReadAsync(userId.Value);

                return Json(new { success = true, message = "Marked all as read" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("CurrentUserId");
                if (!userId.HasValue)
                {
                    return Json(new { success = false, message = "User not logged in" });
                }

                await _notificationService.DeleteAllAsync(userId.Value);

                return Json(new { success = true, message = "Deleted all notifications" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

