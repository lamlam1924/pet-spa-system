using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceService _serviceService;
        private readonly ISerCateService _serCateService;

        public ServiceController(IServiceService serviceService, ISerCateService serCateService)
        {
            _serviceService = serviceService;
            _serCateService = serCateService;
        }

        public IActionResult ListService()
        {
            var services = _serviceService.GetActiveServices();
            var categories = _serCateService.GetActiveCategories();
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