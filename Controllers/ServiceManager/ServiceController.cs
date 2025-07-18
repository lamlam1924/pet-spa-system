using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModels;
using ClosedXML.Excel;

namespace pet_spa_system1.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public IActionResult ListService(int page = 1, int? categoryId = null)
        {
            var filter = new ServiceFilterModel();
            if (categoryId.HasValue)
                filter.CategoryId = categoryId.Value;

            var viewModel = _serviceService.GetServiceListViewModel(filter, page);
            return View(viewModel);
        }

        public IActionResult HomeService()
        {
            return View();
        }

    }
}