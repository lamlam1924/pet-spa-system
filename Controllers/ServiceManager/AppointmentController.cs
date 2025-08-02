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
        public AppointmentController(
            IAppointmentService appointmentService,
            IPetService petService,
            IServiceService serviceService,
            IUserService userService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
            _serviceService = serviceService;
            _userService = userService;
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
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                StartTime = new TimeOnly(9, 0, 0), // Default to 9:00 AM
                User = userEntity
            };

            return View("Appointment", viewModel);
        }

        private void FillViewModel(AppointmentViewModel model, int? userId)
        {
            model.Pets = userId.HasValue ? _petService.GetPetsByUserId(userId.Value) : new List<Pet>();
            model.Services = _serviceService.GetActiveServices().ToList();
            model.Categories = _serviceService.GetAllCategories().ToList();
            model.User = userId.HasValue ? _userService.GetUserInfo(userId.Value) : null;
        }

        // POST: /Appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Appointment(AppointmentViewModel model)
        {
            // Parse StartTime từ StartTimeString nếu có
            if (!string.IsNullOrEmpty(model.StartTimeString))
            {
                if (TimeOnly.TryParse(model.StartTimeString, out var parsedTime))
                {
                    model.StartTime = parsedTime;
                }
                else
                {
                    ModelState.AddModelError("StartTimeString", "Giờ hẹn không hợp lệ. Định dạng phải là HH:mm.");
                }
            }

            int? userId = HttpContext.Session.GetInt32("CurrentUserId");

            if (!ModelState.IsValid)
            {
                FillViewModel(model, userId);
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
                if (userId == null)
                {
                    return RedirectToAction("Login", "Login");
                }

                if (_appointmentService.SaveAppointment(model, userId.Value))
                {
                    TempData["SuccessMessage"] = "Đặt lịch thành công!";
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("Success") });
                    }
                    return RedirectToAction(nameof(Success));
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại.");
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại." });
                }
            }

            FillViewModel(model, userId);
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
        public IActionResult RequestCancel([FromBody] RequestCancelDto dto)
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
                return Json(new { success = true, message = "Yêu cầu hủy lịch đã được gửi, chờ admin duyệt." });
            }
            else
            {
                return Json(new { success = false, message = "Không thể gửi yêu cầu hủy lịch. Vui lòng thử lại." });
            }
        }
        
    }

    public class RequestCancelDto
    {
        public int appointmentId { get; set; }
    }
}
