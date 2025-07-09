using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public IActionResult ListService()
        {
            // Sử dụng IServiceService để lấy ServiceListViewModel
            var viewModel = _serviceService.GetServiceListViewModel(new ServiceFilterModel(), 1);
            
            return View(viewModel);
        }

        public IActionResult HomeService()
        {
            return View();
        }
    }
}