using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pet_spa_system1.Controllers
{
    public class AdminServiceController : Controller
    {
        private readonly IServiceService _serviceService;
        
        // Constants
        private const string ServiceListAction = "ServiceList";
        private const string ServiceCategoryAction = "ServiceCategory";
        private const string ServiceDetailAction = "ServiceDetail";
        private const string SuccessMessageKey = "SuccessMessage";
        private const string ErrorMessageKey = "ErrorMessage";
        private const string InvalidDataMessage = "Dữ liệu không hợp lệ";

        public AdminServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        // SERVICE MANAGEMENT
        // Danh sách dịch vụ
        public IActionResult ServiceList(int? categoryId, string search, string sort, string status, int page = 1)
        {
            // For GET actions with primitive types, just log the error and continue with default values
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Warning: ModelState invalid for ServiceList, but proceeding with request");
            }
            
            var serviceViewModel = _serviceService.GetAllService();
            var services = serviceViewModel.Services.ToList();
            
            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                services = services.Where(s => s.CategoryId == categoryId.Value).ToList();
            }
            
            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active")
                {
                    services = services.Where(s => s.IsActive == true).ToList();
                }
                else if (status == "inactive")
                {
                    services = services.Where(s => s.IsActive != true).ToList();
                }
            }
            
            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(search))
            {
                services = services.Where(s => s.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            // Sắp xếp
            switch (sort)
            {
                case "name_asc":
                    services = services.OrderBy(s => s.Name).ToList();
                    break;
                case "name_desc":
                    services = services.OrderByDescending(s => s.Name).ToList();
                    break;
                case "price_asc":
                    services = services.OrderBy(s => s.Price).ToList();
                    break;
                case "price_desc":
                    services = services.OrderByDescending(s => s.Price).ToList();
                    break;
                case "newest":
                    services = services.OrderByDescending(s => s.ServiceId).ToList();
                    break;
                default:
                    services = services.OrderBy(s => s.Name).ToList();
                    break;
            }
            
            // Remove PageInfo setup since ServiceViewModel doesn't have this property
            // Just update the services with filtered and sorted results
            serviceViewModel.Services = services;
            
            ViewBag.Categories = serviceViewModel.Categories;
            
            return View("~/Views/Admin/ManageService/ServiceList.cshtml", serviceViewModel);
        }
        
        // Chi tiết dịch vụ
        public IActionResult ServiceDetail(int id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(ServiceListAction);
            }
            
            // Lấy thông tin chi tiết dịch vụ qua ViewModel
            var serviceDetailModel = _serviceService.GetServiceDetailData(id);
            if (serviceDetailModel == null || serviceDetailModel.Service == null)
            {
                return NotFound();
            }
            
            // Trả về view với ViewModel đầy đủ
            return View("~/Views/Admin/ManageService/ServiceDetail.cshtml", serviceDetailModel);
        }
        
        // Thêm mới dịch vụ (GET)
        [HttpGet]
        public IActionResult AddService()
        {
            // Tạo mới ServiceFormViewModel chứa cả dịch vụ và danh sách danh mục
            var viewModel = new ServiceFormViewModel
            {
                Service = new Service { IsActive = true, DurationMinutes = 30 },
                Categories = _serviceService.GetAllService().Categories,
                CategoryName = "Chọn danh mục"
            };
            
            return View("~/Views/Admin/ManageService/AddService.cshtml", viewModel);
        }
        
        // Thêm mới dịch vụ (POST)
        [HttpPost]
        public IActionResult AddService(ServiceFormViewModel viewModel)
        {
            var service = viewModel.Service;
            
            // Đảm bảo các giá trị null được xử lý đúng
            if (service.CreatedAt == null)
            {
                service.CreatedAt = DateTime.Now;
            }
            
            // Ensure IsActive has a default value if it's still null
            if (!service.IsActive.HasValue)
            {
                service.IsActive = true; // Default to active if not specified
            }
            
            // Ensure DurationMinutes has a default value if it's still null
            if (!service.DurationMinutes.HasValue)
            {
                service.DurationMinutes = 30; // Default to 30 minutes if not specified
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _serviceService.AddService(service);
                    _serviceService.Save();
                    TempData[SuccessMessageKey] = "Thêm dịch vụ thành công!";
                    return RedirectToAction(ServiceListAction);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu dịch vụ: " + ex.Message);
                }
            }
            
            // Nếu có lỗi, hiển thị lại form với lỗi
            viewModel.Categories = _serviceService.GetAllService().Categories;
            viewModel.CategoryName = "Chọn danh mục";
            return View("~/Views/Admin/ManageService/AddService.cshtml", viewModel);
        }
        
        // Chỉnh sửa dịch vụ (GET)
        [HttpGet]
        public IActionResult EditService(int id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(ServiceListAction);
            }
            
            var service = _serviceService.GetServiceById(id);
            if (service == null)
            {
                return NotFound();
            }
            
            var serviceDetail = _serviceService.GetServiceDetailData(id);
            var category = _serviceService.GetAllService().Categories.FirstOrDefault(c => c.CategoryId == service.CategoryId);
            
            var viewModel = new ServiceFormViewModel
            {
                Service = service,
                Categories = _serviceService.GetAllService().Categories,
                CategoryName = category?.Name ?? "Danh mục dịch vụ",
                ChangeHistory = serviceDetail?.ChangeHistory ?? new List<ServiceChangeHistoryItem>()
            };
            
            return View("~/Views/Admin/ManageService/EditService.cshtml", viewModel);
        }
        
        // Chỉnh sửa dịch vụ (POST)
        [HttpPost]
        public IActionResult EditService(ServiceFormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _serviceService.UpdateService(viewModel.Service);
                _serviceService.Save();
                TempData[SuccessMessageKey] = "Cập nhật dịch vụ thành công!";
                return RedirectToAction(ServiceListAction);
            }
            
            // Nếu có lỗi, hiển thị lại form với lỗi
            viewModel.Categories = _serviceService.GetAllService().Categories;
            var category = _serviceService.GetAllService().Categories.FirstOrDefault(c => c.CategoryId == viewModel.Service.CategoryId);
            viewModel.CategoryName = category?.Name ?? "Danh mục dịch vụ";
            return View("~/Views/Admin/ManageService/EditService.cshtml", viewModel);
        }
        
        // Xóa tạm thời dịch vụ (Soft Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SoftDeleteService(int serviceId)
        {
            try
            {
                if (serviceId <= 0)
                {
                    TempData["ErrorMessage"] = "ID dịch vụ không hợp lệ";
                    return RedirectToAction("ServiceList");
                }
                
                var service = _serviceService.GetServiceById(serviceId);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy dịch vụ";
                    return RedirectToAction("ServiceList");
                }
                
                _serviceService.SoftDeleteService(serviceId);
                _serviceService.Save();
                
                // Fix encoding cho tiếng Việt
                TempData["SuccessMessage"] = "Tạm ngưng dịch vụ thành công";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SoftDeleteService: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạm ngưng dịch vụ";
            }
            
            return RedirectToAction("ServiceList");
        }
        
        // Khôi phục dịch vụ đã xóa tạm thời
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RestoreService(int serviceId)
        {
            try
            {
                if (serviceId <= 0)
                {
                    TempData["ErrorMessage"] = "ID dịch vụ không hợp lệ";
                    return RedirectToAction("ServiceList");
                }
                
                var service = _serviceService.GetServiceById(serviceId);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy dịch vụ";
                    return RedirectToAction("ServiceList");
                }
                
                _serviceService.RestoreService(serviceId);
                _serviceService.Save();
                
                // Fix encoding cho tiếng Việt
                TempData["SuccessMessage"] = "Kích hoạt dịch vụ thành công";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RestoreService: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi kích hoạt dịch vụ";
            }
            
            return RedirectToAction("ServiceList");
        }
        
        // Quản lý danh mục dịch vụ
        public IActionResult ServiceCategory()
        {
            // Sử dụng ServiceCategoryViewModel thay vì ViewBag
            var viewModel = new ServiceCategoryViewModel
            {
                Categories = _serviceService.GetAllCategories(),
                ServiceCountsByCategory = _serviceService.GetServiceCountsByCategory()
            };
            
            return View("~/Views/Admin/ManageService/ServiceCategory.cshtml", viewModel);
        }
        
        // Thêm danh mục dịch vụ
        [HttpPost]
        public IActionResult AddServiceCategory(SerCate category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _serviceService.AddCategory(category);
                    TempData[SuccessMessageKey] = "Đã thêm danh mục dịch vụ thành công!";
                }
                catch (Exception ex)
                {
                    TempData[ErrorMessageKey] = "Lỗi khi thêm danh mục: " + ex.Message;
                }
            }
            
            return RedirectToAction(ServiceCategoryAction);
        }
        
        // Cập nhật danh mục dịch vụ
        [HttpPost]
        public IActionResult UpdateServiceCategory(SerCate category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = _serviceService.GetCategoryById(category.CategoryId);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }
                    
                    _serviceService.UpdateCategory(category);
                    
                    TempData[SuccessMessageKey] = "Đã cập nhật danh mục dịch vụ thành công!";
                }
                catch (Exception ex)
                {
                    TempData[ErrorMessageKey] = "Lỗi khi cập nhật danh mục: " + ex.Message;
                }
            }
            
            return RedirectToAction(ServiceCategoryAction);
        }
        
        // Xóa danh mục dịch vụ
        [HttpPost]
        public IActionResult DeleteServiceCategory(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(InvalidDataMessage);
            }
            
            try
            {
                // Kiểm tra xem danh mục có tồn tại không
                var category = _serviceService.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound();
                }
                
                // Kiểm tra xem danh mục có dịch vụ không
                if (_serviceService.CategoryHasServices(id))
                {
                    TempData[ErrorMessageKey] = "Không thể xóa danh mục này vì có dịch vụ đang sử dụng!";
                    return RedirectToAction(ServiceCategoryAction);
                }
                
                var result = _serviceService.DeleteCategory(id);
                if (result)
                {
                    TempData[SuccessMessageKey] = "Đã xóa danh mục dịch vụ thành công!";
                }
                else
                {
                    TempData[ErrorMessageKey] = "Không thể xóa danh mục.";
                }
            }
            catch (Exception ex)
            {
                TempData[ErrorMessageKey] = "Lỗi khi xóa danh mục: " + ex.Message;
            }
            
            return RedirectToAction(ServiceCategoryAction);
        }
        
        // API cập nhật thứ tự hiển thị danh mục
        [HttpPost]
        public IActionResult UpdateCategoryOrder([FromBody] List<CategoryOrderItem> categories)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            
            try
            {
                if (categories == null || !categories.Any())
                {
                    return Json(new { success = false, message = "Không có dữ liệu danh mục để cập nhật" });
                }
                
                foreach (var item in categories)
                {
                    // Sử dụng ServiceService để cập nhật thứ tự hiển thị
                    var category = _serviceService.GetCategoryById(item.CategoryId);
                    if (category != null)
                    {
                        // Tính năng này sẽ được triển khai sau khi thêm trường DisplayOrder vào model SerCate
                        // Hiện tại chỉ log thông tin
                        Console.WriteLine($"Category {item.CategoryId} would be updated to order {item.DisplayOrder}");
                    }
                }
                _serviceService.Save();
                
                return Json(new { success = true, message = "Đã cập nhật thứ tự danh mục thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi cập nhật thứ tự danh mục: " + ex.Message });
            }
        }
        
        // Xóa dịch vụ hoàn toàn
        [HttpPost]
        public IActionResult DeleteService(int id)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                TempData[ErrorMessageKey] = "ID dịch vụ không hợp lệ";
                return RedirectToAction(ServiceListAction);
            }
            
            try
            {
                var service = _serviceService.GetServiceById(id);
                if (service == null)
                {
                    return NotFound();
                }
                
                // Kiểm tra xem dịch vụ có đang được sử dụng không
                var appointmentServices = _serviceService.GetAppointmentServicesByServiceId(id);
                if (appointmentServices != null && appointmentServices.Any())
                {
                    TempData[ErrorMessageKey] = "Không thể xóa dịch vụ này vì đã có lịch hẹn sử dụng nó!";
                    return RedirectToAction(ServiceDetailAction, new { id });
                }
                
                // Gọi đến service để xóa dịch vụ
                _serviceService.DeleteService(service);
                _serviceService.Save();
                
                TempData[SuccessMessageKey] = "Đã xóa dịch vụ thành công!";
                return RedirectToAction(ServiceListAction);
            }
            catch (Exception ex)
            {
                TempData[ErrorMessageKey] = "Lỗi khi xóa dịch vụ: " + ex.Message;
                return RedirectToAction(ServiceDetailAction, new { id });
            }
        }
        
        // Hiển thị trang index dịch vụ - trang tổng quan
        public IActionResult Index()
        {
            // Removed unnecessary ModelState validation for GET action
            // Sử dụng ViewModel thay vì ViewBag
            var dashboardData = _serviceService.GetServiceDashboardData();
            
            return View("~/Views/Admin/ManageService/Index.cshtml", dashboardData);
        }

        // SERVICE DASHBOARD
        public IActionResult ServiceDashboard()
        {
            var dashboardModel = _serviceService.GetServiceDashboardData();
            
            // Extract data for the charts in a format that doesn't use lambda expressions in the view
            if (dashboardModel.TopServiceBooking != null)
            {
                ViewBag.TopServiceLabels = dashboardModel.TopServiceBooking.Select(kvp => kvp.Value).ToArray();
                ViewBag.TopServiceData = dashboardModel.TopServiceBooking.Select(kvp => kvp.Key).ToArray();
            }
            else
            {
                ViewBag.TopServiceLabels = new string[] { };
                ViewBag.TopServiceData = new int[] { };
            }
            
            if (dashboardModel.CategoryDistribution != null)
            {
                ViewBag.CategoryLabels = dashboardModel.CategoryDistribution.Select(kvp => kvp.Value).ToArray();
                ViewBag.CategoryData = dashboardModel.CategoryDistribution.Select(kvp => kvp.Key).ToArray();
            }
            else
            {
                ViewBag.CategoryLabels = new string[] { };
                ViewBag.CategoryData = new int[] { };
            }
            
            return View("~/Views/Admin/ManageService/ServiceDashboard.cshtml", dashboardModel);
        }

        // Helper class for category ordering
        public class CategoryOrderItem
        {
            public int CategoryId { get; set; }
            public int DisplayOrder { get; set; }
        }
    }
}
