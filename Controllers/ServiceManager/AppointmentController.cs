using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;
using pet_spa_system1.Services;

namespace pet_spa_system1.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;
        private readonly IServiceService _serviceService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public AppointmentController(
            IAppointmentService appointmentService,
            IPetService petService,
            IServiceService serviceService,
            IUserService userService,
            INotificationService notificationService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
            _serviceService = serviceService;
            _userService = userService;
            _notificationService = notificationService;
        }

        // GET: /Appointment
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                // Xử lý trường hợp chưa đăng nhập, ví dụ:
                return RedirectToAction("Login", "Login");
            }

            var userEntity = _userService.GetUserInfo(userId.Value);
            var viewModel = new AppointmentViewModel
            {
                Pets = _petService.GetPetsByUserId(userId.Value),
                Services = _serviceService.GetActiveServices().ToList(),
                Categories = _serviceService.GetAllCategories().ToList(),
                AppointmentDate = DateTime.Today.AddDays(1),
                AppointmentTime = new TimeSpan(9, 0, 0), // Default to 9:00 AM
                User = userEntity
            };

            return View("Appointment", viewModel);
        }

        // POST: /Appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AppointmentAsync(AppointmentViewModel model)
        {

            if (!ModelState.IsValid)
            {
                int? userId = HttpContext.Session.GetInt32("CurrentUserId");
                if (userId != null)
                {
                    model.Pets = _petService.GetPetsByUserId(userId.Value);
                }
                else
                {
                    model.Pets = new List<Pet>();
                }
                model.Services = _serviceService.GetActiveServices().ToList();
                model.Categories = _serviceService.GetAllCategories().ToList();
                // Lấy lại user cho viewmodel nếu cần
                int? userIdForUser = HttpContext.Session.GetInt32("CurrentUserId");
                if (userIdForUser != null)
                {
                    model.User = _userService.GetUserInfo(userIdForUser.Value);
                }
                // Return JSON for AJAX requests
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, errors });
                }
                return View(model);
            }

            try
            {
                int? userId = HttpContext.Session.GetInt32("CurrentUserId");
                if (userId == null)
                {
                    // Xử lý trường hợp chưa đăng nhập, ví dụ:
                    return RedirectToAction("Login", "Login");
                }

                if (_appointmentService.SaveAppointment(model, userId.Value))
                {
                    // Tạo thông báo khi đặt lịch thành công
                    var notification = new Notification
                    {
                        UserId = userId.Value,
                        Title = "Đặt lịch thành công",
                        Message = $"Lịch hẹn của bạn đã được đặt thành công. Chúng tôi sẽ xác nhận sớm nhất.",
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    };
                    
                    await _notificationService.AddAsync(notification);
                    
                    TempData["SuccessMessage"] = "Đặt lịch thành công!";
                    // Return JSON for AJAX requests
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("Success") });
                    }
                    return RedirectToAction(nameof(Success));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại.");

                // Return JSON for AJAX requests
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại." });
                }
            }

            int? userIdFinal = HttpContext.Session.GetInt32("CurrentUserId");
            if (userIdFinal != null)
            {
                model.Pets = _petService.GetPetsByUserId(userIdFinal.Value);
                model.User = _userService.GetUserInfo(userIdFinal.Value);
            }
            else
            {
                model.Pets = new List<Pet>();
                model.User = null;
            }
            model.Services = _serviceService.GetActiveServices().ToList();
            model.Categories = _serviceService.GetAllCategories().ToList();
            return View(model);
        }

        // GET: /Appointment/Success
        public IActionResult Success()
        {
            // Pass success message to view if it exists
            ViewBag.SuccessMessage = TempData["SuccessMessage"]?.ToString();
            return View();
        }

        // GET: /Appointment/History
        public IActionResult History()
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                // Xử lý trường hợp chưa đăng nhập, ví dụ:
                return RedirectToAction("Login", "Login");
            }
            var model = _appointmentService.GetAppointmentHistory(userId.Value);
            return View("History", model);
        }

        // POST: /Appointment/RequestCancel
        [HttpPost]
        public async Task<IActionResult> RequestCancelAsync([FromBody] RequestCancelDto dto)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                // Xử lý trường hợp chưa đăng nhập, ví dụ:
                return RedirectToAction("Login", "Login");
            }

            Console.WriteLine($"[RequestCancel] appointmentId nhận được từ client: {dto?.appointmentId}");

            var result = _appointmentService.RequestCancelAppointment(dto?.appointmentId ?? 0, userId.Value);

            if (result)
            {
                // Tạo thông báo khi gửi yêu cầu hủy lịch thành công
                var notification = new Notification
                {
                    UserId = userId.Value,
                    Title = "Yêu cầu hủy lịch",
                    Message = $"Yêu cầu hủy lịch hẹn đã được gửi. Chúng tôi sẽ xử lý trong thời gian sớm nhất.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };
                
                await _notificationService.AddAsync(notification);
                
                return Json(new { success = true, message = "Yêu cầu hủy lịch đã được gửi, chờ admin duyệt." });
            }
            else
            {
                return Json(new { success = false, message = "Không thể gửi yêu cầu hủy lịch. Vui lòng thử lại." });
            }
        }

        // GET: /Appointment/Detail/{id}
        [HttpGet]
        public IActionResult Detail(int id)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để xem chi tiết lịch hẹn." });
            }
            var detail = _appointmentService.GetAppointmentDetailWithPetImages(id, userId.Value);
            if (detail == null)
            {
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn." });
            }
            return Json(new { success = true, data = detail });
        }

        public IActionResult AppointmentDetail()
        {
            return View();
        }

    }

    public class RequestCancelDto
    {
        public int appointmentId { get; set; }
    }
    
}
