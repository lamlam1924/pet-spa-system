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

