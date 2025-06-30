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

            // DEBUG
            Console.WriteLine($"Nhận được {model.SelectedServiceIds?.Count ?? 0} dịch vụ");
            if (model.SelectedServiceIds != null)
            {
                foreach (var serviceId in model.SelectedServiceIds)
                {
                    Console.WriteLine($"Service ID: {serviceId}");
                }
            }
            
            // Kiểm tra xem có đủ dịch vụ và thú cưng đã chọn chưa
            if (!ModelState.IsValid || model.SelectedServiceIds == null || !model.SelectedServiceIds.Any() ||
                model.SelectedPetIds == null || !model.SelectedPetIds.Any())
            {
                // Kiểm tra thông tin thú cưng và dịch vụ trước khi tiếp tục
                if (model.SelectedServiceIds == null || !model.SelectedServiceIds.Any())
                {
                    ModelState.AddModelError("", "Vui lòng chọn ít nhất một dịch vụ.");
                }
                
                if (model.SelectedPetIds == null || !model.SelectedPetIds.Any())
                {
                    ModelState.AddModelError("", "Vui lòng chọn ít nhất một thú cưng.");
                }
                
                // In ra debug để kiểm tra
                Console.WriteLine($"Validation failed: Services={model.SelectedServiceIds?.Count ?? 0}, Pets={model.SelectedPetIds?.Count ?? 0}");
                
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

            // Đặt lịch thành công - tạo ViewModel mới để tránh các lỗi từ form cũ
            ViewBag.BookingSuccess = true;
            
            // Debug thông tin thú cưng đã chọn
            if (model.SelectedPetIds != null)
            {
                foreach (var petId in model.SelectedPetIds) 
                {
                    Console.WriteLine($"Pet ID được chọn: {petId}");
                }
            }
            
            var newModel = new AppointmentViewModel
            {
                Services = _serviceService.GetActiveServices(),
                Categories = _serCateService.GetActiveCategories(),
                Pets = _petService.GetPetsByUserId(userId),
                CustomerName = model.CustomerName, // giữ lại thông tin khách hàng để tiện cho lần đặt tiếp theo
                Phone = model.Phone,
                Email = model.Email
            };
            return View("Appointment", newModel);
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
        
        [HttpGet]
        public IActionResult EmailForAppointment()
        {
            // TODO: sau này thay 1 bằng userId thực tế từ đăng nhập
            int userId = 2;
            //var appointments = _appointmentService.GetAppointmentsByUser(userId);
            var viewmodel = _appointmentService.GetAppointmentHistory(userId);
            
            return View(viewmodel);
        }

    }
}
