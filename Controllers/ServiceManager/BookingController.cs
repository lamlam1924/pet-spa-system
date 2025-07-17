//using Microsoft.AspNetCore.Mvc;
//using pet_spa_system1.Models;
//using pet_spa_system1.Services;

//namespace pet_spa_system1.Controllers
//{
//    public class BookingController : Controller
//    {
//        private readonly IPetService _petService;
//        private readonly IServiceService _serviceService;
//        private readonly ICategoryService _categoryService;

//        public BookingController(IPetService petService, IServiceService serviceService, ICategoryService categoryService)
//        {
//            _petService = petService;
//            _serviceService = serviceService;
//            _categoryService = categoryService;
//        }

//        public IActionResult Booking()
//        {
//            var userId = 1; // giả lập, thực tế lấy từ User.Identity
//            var pets = _petService.GetPetsByUserId(userId);
//            var services = _serviceService.GetActiveServices();
//            var categories = _categoryService.GetActiveCategories();

//            var model = new BookingViewModel
//            {
//                Pets = pets ?? new List<Pet>(),
//                Services = services ?? new List<Service>(),
//                Categories = categories ?? new List<SerCate>()
//            };
//            return View("~/Views/Service/Booking", model);
//        }

//        // Có thể bổ sung các action khác liên quan đến đặt lịch ở đây
//    }
//}
