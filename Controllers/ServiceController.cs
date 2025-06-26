using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;

namespace pet_spa_system1.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceService _serviceService;
        private readonly ICategoryService _categoryService;

        public ServiceController(IServiceService serviceService, ICategoryService categoryService)
        {
            _serviceService = serviceService;
            _categoryService = categoryService;
        }

        public IActionResult ListService()
        {
            var services = _serviceService.GetActiveServices();
            var categories = _categoryService.GetActiveCategories();
            // Trả về view với model phù hợp
            return View(new ServiceViewModel
            {
                Services = services,
                Categories = categories
            });
        }

        public IActionResult HomeService()
        {
            return View();
        }
        
    }
}