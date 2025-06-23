using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;

namespace pet_spa_system1.Controllers
{
    public class AdminController : Controller
    {
        private readonly IServiceService _serviceService;

        public AdminController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public IActionResult ManageService(int? categoryId, string search)
        {
            var model = _serviceService.GetAllService();
            if (categoryId.HasValue)
            {
                model.Services = model.Services.Where(s => s.CategoryId == categoryId.Value).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                model.Services = model.Services.Where(s => s.Name.Contains(search)).ToList();
            }
            model.SelectedCategoryId = categoryId;
            return View(model);
        }

        [HttpPost]
        public IActionResult AddService(Service service)
        {
            _serviceService.AddService(service);
            _serviceService.Save();
            return RedirectToAction("ManageService");
        }

        public IActionResult EditService(int id)
        {
            var service = _serviceService.GetServiceById(id);
            var categories = _serviceService.GetAllService().Categories;
            ViewBag.Categories = categories;
            return View(service);
        }

        [HttpPost]
        public IActionResult EditService(Service service)
        {
            _serviceService.UpdateService(service);
            _serviceService.Save();
            return RedirectToAction("ManageService");
        }

        public IActionResult SoftDeleteService(int id)
        {
            _serviceService.SoftDeleteService(id);
            _serviceService.Save();
            return RedirectToAction("ManageService");
        }

        public IActionResult RestoreService(int id)
        {
            _serviceService.RestoreService(id);
            _serviceService.Save();
            return RedirectToAction("ManageService");
        }

        public IActionResult Appointments()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Payment()
        {
            return View();
        }
        public IActionResult Pets_List()
        {
            return View();
        }
        public IActionResult List_Customer()
        {
            return View();
        }
        public IActionResult Product_Detail()
        {
            return View();
        }

        public IActionResult Product_List()
        {
            return View();
        }
        public IActionResult Refund()
        {
            return View();
        }

        public IActionResult Staff()
        {
            return View();
        }
        
    }
}
