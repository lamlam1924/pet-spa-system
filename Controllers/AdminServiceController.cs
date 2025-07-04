using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pet_spa_system1.Controllers
{
    public class AdminServiceController : Controller
    {
        private readonly PetDataShopContext _context;
        private readonly IServiceService _serviceService;

        public AdminServiceController(PetDataShopContext context, IServiceService serviceService)
        {
            _context = context;
            _serviceService = serviceService;
        }

        // SERVICE MANAGEMENT
        // Danh sách dịch vụ
        public IActionResult ServiceList(int? categoryId, string search, string sort, string status, int page = 1)
        {
            var model = _serviceService.GetAllService();
            
            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                model.Services = model.Services.Where(s => s.CategoryId == categoryId.Value).ToList();
            }
            
            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active")
                {
                    model.Services = model.Services.Where(s => s.IsActive==true).ToList();
                }
                else if (status == "inactive")
                {
                    model.Services = model.Services.Where(s => !s.IsActive==true).ToList();
                }
            }
            
            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(search))
            {
                model.Services = model.Services.Where(s => s.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            // Sắp xếp
            switch (sort)
            {
                case "name_asc":
                    model.Services = model.Services.OrderBy(s => s.Name).ToList();
                    break;
                case "name_desc":
                    model.Services = model.Services.OrderByDescending(s => s.Name).ToList();
                    break;
                case "price_asc":
                    model.Services = model.Services.OrderBy(s => s.Price).ToList();
                    break;
                case "price_desc":
                    model.Services = model.Services.OrderByDescending(s => s.Price).ToList();
                    break;
                default:
                    model.Services = model.Services.OrderBy(s => s.ServiceId).ToList();
                    break;
            }
            
            return View("~/Views/Admin/ManageService/ServiceList.cshtml", model);
        }
        
        // Chi tiết dịch vụ
        public IActionResult ServiceDetail(int id)
        {
            var service = _serviceService.GetServiceById(id);
            if (service == null)
            {
                return NotFound();
            }
            
            // Thống kê đặt lịch
            var appointmentServices = _context.AppointmentServices
                .Include(a => a.Appointment)
                .ThenInclude(a => a.User)
                .Include(a => a.Appointment.Status)
                .Where(a => a.ServiceId == id)
                .OrderByDescending(a => a.Appointment.AppointmentDate)
                .ToList();
                
            ViewBag.AppointmentHistory = appointmentServices.Take(10).ToList();
            
            // Thống kê cơ bản
            ViewBag.BookingCount = appointmentServices.Count;
            ViewBag.Revenue = appointmentServices.Sum(a => service.Price);
            ViewBag.CustomerCount = appointmentServices.Select(a => a.Appointment.UserId).Distinct().Count();
            
            // Các dịch vụ liên quan (cùng danh mục)
            ViewBag.RelatedServices = _serviceService.GetAllService()
                .Services.Where(s => s.CategoryId == service.CategoryId && s.ServiceId != service.ServiceId)
                .Take(4)
                .ToList();
                
            // Lịch hẹn sắp tới
            ViewBag.UpcomingAppointments = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.AppointmentServices)
                .Where(a => a.AppointmentServices.Any(s => s.ServiceId == id) && 
                          a.AppointmentDate > DateTime.Now &&
                          a.StatusId != 4) // Không phải trạng thái đã hủy
                .OrderBy(a => a.AppointmentDate)
                .Take(5)
                .ToList();
                
            return View("~/Views/Admin/ManageService/ServiceDetail.cshtml", service);
        }
        
        // Thêm mới dịch vụ (GET)
        [HttpGet]
        public IActionResult AddService()
        {
            ViewBag.Categories = _serviceService.GetAllService().Categories;
            ViewBag.CategoryName = "Chọn danh mục"; // Add default category name for new services
            return View("~/Views/Admin/ManageService/AddService.cshtml", new Service { IsActive = true });
        }
        
        // Thêm mới dịch vụ (POST)
        [HttpPost]
        public IActionResult AddService(Service service)
        {
            _serviceService.AddService(service);
            _serviceService.Save();
            return RedirectToAction("ServiceList");
        }
        
        // Chỉnh sửa dịch vụ (GET)
        [HttpGet]
        public IActionResult EditService(int id)
        {
            var service = _serviceService.GetServiceById(id);
            if (service == null)
            {
                return NotFound();
            }
            
            ViewBag.Categories = _serviceService.GetAllService().Categories;
            
            // Add the category name directly to ViewBag to avoid lambda in view
            var category = _serviceService.GetAllService().Categories.FirstOrDefault(c => c.CategoryId == service.CategoryId);
            ViewBag.CategoryName = category?.Name ?? "Danh mục dịch vụ";
            
            // Giả lập lịch sử thay đổi (trong thực tế nên lấy từ database)
            var changeHistory = new List<object>
            {
                new 
                {
                    ChangeDate = DateTime.Now.AddDays(-5),
                    UserName = "Admin",
                    ChangeDescription = "Thay đổi giá dịch vụ từ 250,000đ thành 300,000đ"
                },
                new 
                {
                    ChangeDate = DateTime.Now.AddDays(-10),
                    UserName = "Manager",
                    ChangeDescription = "Cập nhật mô tả dịch vụ"
                }
            };
            
            ViewBag.ChangeHistory = changeHistory;
            
            return View("~/Views/Admin/ManageService/EditService.cshtml", service);
        }
        
        // Chỉnh sửa dịch vụ (POST)
        [HttpPost]
        public IActionResult EditService(Service service)
        {
            _serviceService.UpdateService(service);
            _serviceService.Save();
            return RedirectToAction("ServiceList");
        }
        
        // Xóa tạm thời dịch vụ (Soft Delete)
        public IActionResult SoftDeleteService(int id)
        {
            _serviceService.SoftDeleteService(id);
            _serviceService.Save();
            return RedirectToAction("ServiceList");
        }
        
        // Khôi phục dịch vụ đã xóa tạm thời
        public IActionResult RestoreService(int id)
        {
            _serviceService.RestoreService(id);
            _serviceService.Save();
            return RedirectToAction("ServiceList");
        }
        
        // Quản lý danh mục dịch vụ
        public IActionResult ServiceCategory()
        {
            var categories = _serviceService.GetAllService().Categories.ToList();
            
            // Thống kê số lượng dịch vụ theo danh mục
            var serviceCounts = _context.Services
                .GroupBy(s => s.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionary(x => x.CategoryId, x => x.Count);
                
            ViewBag.ServiceCounts = serviceCounts;
            
            return View("~/Views/Admin/ManageService/ServiceCategory.cshtml", categories);
        }
        
        // Thêm danh mục dịch vụ
        [HttpPost]
        public IActionResult AddServiceCategory(SerCate category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    category.CreatedAt = DateTime.Now;
                    _context.SerCates.Add(category);
                    _context.SaveChanges();
                    
                    TempData["SuccessMessage"] = "Đã thêm danh mục dịch vụ thành công!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi khi thêm danh mục: " + ex.Message;
                }
            }
            
            return RedirectToAction("ServiceCategory");
        }
        
        // Cập nhật danh mục dịch vụ
        [HttpPost]
        public IActionResult UpdateServiceCategory(SerCate category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingCategory = _context.SerCates.Find(category.CategoryId);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }
                    
                    existingCategory.Name = category.Name;
                    existingCategory.Description = category.Description;
                    existingCategory.IsActive = category.IsActive;
                    
                    _context.SerCates.Update(existingCategory);
                    _context.SaveChanges();
                    
                    TempData["SuccessMessage"] = "Đã cập nhật danh mục dịch vụ thành công!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi khi cập nhật danh mục: " + ex.Message;
                }
            }
            
            return RedirectToAction("ServiceCategory");
        }
        
        // Xóa danh mục dịch vụ
        [HttpPost]
        public IActionResult DeleteServiceCategory(int id)
        {
            try
            {
                var category = _context.SerCates.Find(id);
                if (category == null)
                {
                    return NotFound();
                }
                
                // Kiểm tra xem danh mục có dịch vụ không
                var hasServices = _context.Services.Any(s => s.CategoryId == id);
                if (hasServices)
                {
                    TempData["ErrorMessage"] = "Không thể xóa danh mục này vì có dịch vụ đang sử dụng!";
                    return RedirectToAction("ServiceCategory");
                }
                
                _context.SerCates.Remove(category);
                _context.SaveChanges();
                
                TempData["SuccessMessage"] = "Đã xóa danh mục dịch vụ thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa danh mục: " + ex.Message;
            }
            
            return RedirectToAction("ServiceCategory");
        }
        
        // API cập nhật thứ tự hiển thị danh mục
        [HttpPost]
        public IActionResult UpdateCategoryOrder([FromBody] List<CategoryOrderItem> categories)
        {
            try
            {
                foreach (var item in categories)
                {
                    var category = _context.SerCates.Find(item.CategoryId);
                    if (category != null)
                    {
                        // Trong thực tế, bạn cần thêm trường DisplayOrder vào model SerCate
                        // category.DisplayOrder = item.DisplayOrder;
                        _context.SerCates.Update(category);
                    }
                }
                _context.SaveChanges();
                
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
            try
            {
                var service = _serviceService.GetServiceById(id);
                if (service == null)
                {
                    return NotFound();
                }
                
                // Kiểm tra xem dịch vụ có đang được sử dụng không
                var isInUse = _context.AppointmentServices.Any(a => a.ServiceId == id);
                if (isInUse)
                {
                    TempData["ErrorMessage"] = "Không thể xóa dịch vụ này vì đã có lịch hẹn sử dụng nó!";
                    return RedirectToAction("ServiceDetail", new { id });
                }
                
                _context.Services.Remove(service);
                _context.SaveChanges();
                
                TempData["SuccessMessage"] = "Đã xóa dịch vụ thành công!";
                return RedirectToAction("ServiceList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa dịch vụ: " + ex.Message;
                return RedirectToAction("ServiceDetail", new { id });
            }
        }
        
        // Hiển thị trang index dịch vụ - trang tổng quan
        public IActionResult Index()
        {
            // Thống kê tổng số dịch vụ
            var allServices = _context.Services.ToList();
            ViewBag.TotalServices = allServices.Count;
            ViewBag.ActiveServices = allServices.Count(s => s.IsActive == true);
            ViewBag.InactiveServices = allServices.Count(s => s.IsActive != true);
            
            // Thống kê danh mục
            var categories = _context.SerCates.ToList();
            ViewBag.TotalCategories = categories.Count;
            
            // Dịch vụ mới thêm gần đây
            ViewBag.RecentServices = _context.Services
                .Include(s => s.Category)
                .OrderByDescending(s => s.CreatedAt)
                .Take(5)
                .ToList();
                
            // Lịch hẹn sắp tới
            ViewBag.UpcomingAppointments = _context.Appointments
                .Include(a => a.User)
                .Include(a => a.AppointmentServices)
                .ThenInclude(a => a.Service)
                .Include(a => a.Status)
                .Where(a => a.AppointmentDate > DateTime.Now && a.StatusId != 4)
                .OrderBy(a => a.AppointmentDate)
                .Take(5)
                .ToList();
                
            // Dữ liệu cho biểu đồ top 10 dịch vụ được đặt nhiều nhất
            var topServices = _context.AppointmentServices
                .GroupBy(a => a.ServiceId)
                .Select(g => new { ServiceId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();
                
            var serviceBookingData = new
            {
                labels = topServices.Select(s => 
                {
                    var serviceName = _context.Services.FirstOrDefault(x => x.ServiceId == s.ServiceId)?.Name ?? "";
                    return serviceName.Length > 15 ? serviceName.Substring(0, 15) + "..." : serviceName;
                }).ToList(),
                data = topServices.Select(s => s.Count).ToList()
            };
            
            ViewBag.ServiceBookingData = serviceBookingData;
            
            // Dữ liệu cho biểu đồ phân bố dịch vụ theo danh mục
            var categoryDistribution = _context.Services
                .GroupBy(s => s.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();
                
            var categoryDistributionData = new
            {
                labels = categoryDistribution.Select(c => 
                {
                    var categoryName = _context.SerCates.FirstOrDefault(x => x.CategoryId == c.CategoryId)?.Name ?? "";
                    return categoryName.Length > 15 ? categoryName.Substring(0, 15) + "..." : categoryName;
                }).ToList(),
                data = categoryDistribution.Select(c => c.Count).ToList()
            };
            
            ViewBag.CategoryDistributionData = categoryDistributionData;
            
            return View("~/Views/Admin/ManageService/ServiceDashboard.cshtml");
        }

        // Helper class for category ordering
        public class CategoryOrderItem
        {
            public int CategoryId { get; set; }
            public int DisplayOrder { get; set; }
        }
    }
}
