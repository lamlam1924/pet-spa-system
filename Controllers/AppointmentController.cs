using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;
        private readonly IServiceService _serviceService;

        public AppointmentController(
            IAppointmentService appointmentService,
            IPetService petService,
            IServiceService serviceService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
            _serviceService = serviceService;
        }

        // GET: /Appointment
        public IActionResult Index()
        {
            // Tạm thời hardcode userId = 2
            int userId = 2;

            var viewModel = new AppointmentViewModel
            {
                Pets = _petService.GetAllPets(),
                Services = _serviceService.GetActiveServices().ToList(),
                AppointmentDate = DateTime.Today.AddDays(1),
                AppointmentTime = new TimeSpan(9, 0, 0) // Default to 9:00 AM
            };

            return View("Appointment", viewModel);
        }

        // POST: /Appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Appointment(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Pets = _petService.GetAllPets();
                model.Services = _serviceService.GetActiveServices().ToList();
                
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
                // Tạm thời hardcode userId = 2
                int userId = 2;

                if (_appointmentService.SaveAppointment(model, userId))
                {
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

            model.Pets = _petService.GetAllPets();
            model.Services = _serviceService.GetActiveServices().ToList();
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
            // Tạm thời hardcode userId = 2
            int userId = 2;
            var model = _appointmentService.GetAppointmentHistory(userId);
            return View("History", model);
        }

        // POST: /Appointment/RequestCancel
        [HttpPost]
        public IActionResult RequestCancel([FromBody] RequestCancelDto dto)
        {
            // Tạm thời hardcode userId = 2, thực tế lấy từ session hoặc identity
            int userId = 2;

            Console.WriteLine($"[RequestCancel] appointmentId nhận được từ client: {dto?.appointmentId}");

            var result = _appointmentService.RequestCancelAppointment(dto?.appointmentId ?? 0, userId);

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

