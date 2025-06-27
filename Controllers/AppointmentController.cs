using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;

namespace pet_spa_system1.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IServiceService _serviceService;
        private readonly ISerCateService _serCateService;
        private readonly IUserService _userService;
        private readonly IPetService _petService;

        public AppointmentController(
            IAppointmentService appointmentService,
            IServiceService serviceService,
            ISerCateService serCateService,
            IUserService userService,
            IPetService petService
        )
        {
            _appointmentService = appointmentService;
            _serviceService = serviceService;
            _serCateService = serCateService;
            _userService = userService;
            _petService = petService;
        }

        [HttpGet]
        public IActionResult Book()
        {
            int userId = 2; // Sau này lấy từ User.Identity
            //var user = _userService.GetById(userId);
            var user = _userService.GetUserInfo(userId);

            var model = new AppointmentViewModel
            {
                Services = _serviceService.GetActiveServices(),
                Categories = _serCateService.GetActiveCategories(),
                Pets = _petService.GetPetsByUserId(userId),

                // Auto-fill thông tin user nếu có
                CustomerName = user?.FullName ?? "",
                Phone = user?.Phone ?? "",
                Email = user?.Email ?? ""
            };

            return View("Appointment", model);
        }

        [HttpPost]       
        [ValidateAntiForgeryToken]
        public IActionResult Book(AppointmentViewModel model)
        {
            int userId = 2; // sau này lấy từ User.Identity

            // Debug: Kiểm tra binding
            System.Diagnostics.Debug.WriteLine("ServiceIds: " + string.Join(",", model.SelectedServiceIds));
            System.Diagnostics.Debug.WriteLine("PetIds: " + string.Join(",", model.SelectedPetIds));

            if (!ModelState.IsValid)
            {
                // Nạp lại danh sách cho ViewModel khi có lỗi
                model.Services = _serviceService.GetActiveServices();
                model.Categories = _serCateService.GetActiveCategories();
                model.Pets = _petService.GetPetsByUserId(userId);

                return View("Appointment", model);
            }

            // Gọi Service lưu lịch hẹn
            var result = _appointmentService.SaveAppointment(model, userId);

            if (!result)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu lịch hẹn.");
                // Nạp lại dữ liệu như trên
                model.Services = _serviceService.GetActiveServices();
                model.Categories = _serCateService.GetActiveCategories();
                model.Pets = _petService.GetPetsByUserId(userId);

                return View("Appointment", model);
            }

            ViewBag.BookingSuccess = true;
            model.Services = _serviceService.GetActiveServices();
            model.Categories = _serCateService.GetActiveCategories();
            model.Pets = _petService.GetPetsByUserId(userId);
            return View("Appointment", model);
            // -----------------------------------------------
        }
        
        [HttpGet]
        public IActionResult History()
        {
            // TODO: sau này thay 1 bằng userId thực tế từ đăng nhập
            int userId = 2;
            //var appointments = _appointmentService.GetAppointmentsByUser(userId);
            var viewmodel = _appointmentService.GetAppointmentHistory(userId);
            
            return View(viewmodel);
        }

    }
}
