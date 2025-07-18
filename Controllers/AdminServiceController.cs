using ClosedXML.Excel;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
       
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Controllers
{
    public class AdminServiceController : Controller
    {
        private readonly IServiceService _serviceService;
        private readonly ILogger<AdminServiceController> _logger;

        public AdminServiceController(IServiceService serviceService, ILogger<AdminServiceController> logger)
        {
            _serviceService = serviceService;
            _logger = logger;
        }

        // ===== SERVICE LIST - Pure ViewModel =====
        public IActionResult ServiceList(ServiceFilterModel filter, int page = 1)
        {
            try
            {
                // ✅ Pass page parameter to service
                var viewModel = _serviceService.GetServiceListViewModel(filter, page);
                
                return View("~/Views/Admin/ManageService/ServiceList.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải danh sách dịch vụ");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách dịch vụ.";
                
                var emptyModel = new ServiceListViewModel
                {
                    Services = new List<ServiceListItem>(),
                    Categories = new List<SerCate>(),
                    Filter = filter ?? new ServiceFilterModel(),
                    Pagination = new PaginationModel { CurrentPage = page, PageSize = 10, TotalItems = 0 },
                    Summary = new ServiceSummaryStats()
                };
                
                return View("~/Views/Admin/ManageService/ServiceList.cshtml", emptyModel);
            }
        }

        // ===== SERVICE DASHBOARD - UNIFIED VERSION =====
        public IActionResult ServiceDashboard()
        {
            try
            {
                var dashboardModel = _serviceService.GetServiceDashboardViewModel();
                // Trả về view dashboard cùng dữ liệu tổng hợp
                return View("~/Views/Admin/ManageService/ServiceDashboard.cshtml", dashboardModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải dashboard dịch vụ");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải dashboard.";
                var emptyDashboard = new ServiceDashboardViewModel();
                return View("~/Views/Admin/ManageService/ServiceDashboard.cshtml", emptyDashboard);
            }
        }

        // ===== SERVICE DETAIL - ViewModel =====
        public IActionResult ServiceDetail(int id)
        {
            try
            {
                var viewModel = _serviceService.GetServiceDetailViewModel(id);
                if (viewModel?.Service == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy dịch vụ.";
                    return RedirectToAction("ServiceList");
                }

                return View("~/Views/Admin/ManageService/ServiceDetail.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải chi tiết dịch vụ: {ServiceId}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin dịch vụ.";
                return RedirectToAction("ServiceList");
            }
        }

        // ===== EDIT SERVICE - ViewModel =====
        [HttpGet]
        public IActionResult EditService(int id)
        {
            try
            {
                var service = _serviceService.GetServiceById(id);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy dịch vụ cần chỉnh sửa.";
                    return RedirectToAction("ServiceList");
                }

                var categories = _serviceService.GetAllCategories();
                var selectedCategory = categories.FirstOrDefault(c => c.CategoryId == service.CategoryId);

                var viewModel = new ServiceFormViewModel
                {
                    Input = new ServiceInputViewModel
                    {
                        ServiceId = service.ServiceId,
                        Name = service.Name,
                        Price = service.Price,
                        CategoryId = service.CategoryId,
                        Description = service.Description,
                        DurationMinutes = service.DurationMinutes,
                        IsActive = service.IsActive,
                        CreatedAt = service.CreatedAt,
                        ImageUrl = service.ImageUrl
                    },
                    Categories = categories,
                    CategoryName = selectedCategory?.Name ?? "Chưa chọn danh mục"
                };

                return View("~/Views/Admin/ManageService/EditService.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải form chỉnh sửa dịch vụ: {ServiceId}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form chỉnh sửa.";
                return RedirectToAction("ServiceList");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(ServiceFormViewModel model, IFormFile ImageFile)
        {
            _logger.LogInformation("EditService POST: ModelState.IsValid = {IsValid}", ModelState.IsValid);
            _logger.LogInformation("EditService POST: ServiceId = {Id}, Name = {Name}, Price = {Price}", 
                model.Input.ServiceId, model.Input.Name, model.Input.Price);


            // Nếu dịch vụ chưa có ảnh và không upload ảnh mới, báo lỗi
            var input = model.Input;
            var existingService = _serviceService.GetServiceById(input.ServiceId ?? 0);
            if (!ModelState.IsValid || (string.IsNullOrEmpty(existingService?.ImageUrl) && (ImageFile == null || ImageFile.Length == 0)))
            {
                if (string.IsNullOrEmpty(existingService?.ImageUrl) && (ImageFile == null || ImageFile.Length == 0))
                {
                    ModelState.AddModelError("ImageFile", "Vui lòng chọn ảnh cho dịch vụ.");
                }
                _logger.LogWarning("EditService POST: Model validation failed");
                foreach (var error in ModelState)
                {
                    _logger.LogWarning("Model error in {Key}: {Errors}", error.Key, string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage)));
                }
                try
                {
                    model.Categories = _serviceService.GetAllCategories();
                    TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin đã nhập.";
                    return View("~/Views/Admin/ManageService/EditService.cshtml", model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi reload categories");
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form.";
                    return RedirectToAction("ServiceList");
                }
            }

            try
            {
                if (existingService == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy dịch vụ cần cập nhật.";
                    return RedirectToAction("ServiceList");
                }

                // Xử lý upload ảnh nếu có file mới
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var account = new CloudinaryDotNet.Account(
                        "dprp1jbd9", // cloud_name
                        "584135338254938", // api_key
                        "QbUYngPIdZcXEn_mipYn8RE5dlo" // api_secret
                    );
                    var cloudinary = new CloudinaryDotNet.Cloudinary(account);
                    var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
                    {
                        File = new FileDescription(ImageFile.FileName, ImageFile.OpenReadStream()),
                        Folder = "pet-spa/services"
                    };
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        existingService.ImageUrl = uploadResult.SecureUrl.ToString();
                    }
                }

                // Update properties
                existingService.Name = input.Name?.Trim() ?? string.Empty;
                existingService.CategoryId = input.CategoryId ?? 0;
                existingService.Price = input.Price ?? 0;
                existingService.DurationMinutes = input.DurationMinutes ?? 0;
                existingService.Description = input.Description?.Trim() ?? string.Empty;
                existingService.IsActive = input.IsActive ?? true;

                _serviceService.UpdateService(existingService);
                _serviceService.Save();

                TempData["SuccessMessage"] = "Dịch vụ đã được cập nhật thành công!";

                // Handle SaveAction
                string saveAction = Request.Form["SaveAction"];
                return saveAction switch
                {
                    "SaveAndList" => RedirectToAction("ServiceList"),
                    "SaveAndNew" => RedirectToAction("AddService"),
                    "SaveAndContinue" => RedirectToAction("EditService", new { id = existingService.ServiceId }),
                    _ => RedirectToAction("EditService", new { id = existingService.ServiceId })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật dịch vụ: {ServiceId}", model.Input.ServiceId);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật dịch vụ: " + ex.Message;

                try
                {
                    model.Categories = _serviceService.GetAllCategories();
                }
                catch
                {
                    model.Categories = new List<SerCate>();
                }

                return View("~/Views/Admin/ManageService/EditService.cshtml", model);
            }
        }

        // ===== ADD SERVICE =====
        [HttpGet]
        public IActionResult AddService()
        {
            try
            {
                var viewModel = new ServiceFormViewModel
                {
                    Input = new ServiceInputViewModel { IsActive = true, DurationMinutes = 30 },
                    Categories = _serviceService.GetAllCategories(),
                    CategoryName = "Chọn danh mục"
                };
                
                return View("~/Views/Admin/ManageService/AddService.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải form thêm dịch vụ");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form.";
                return RedirectToAction("ServiceList");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
public async Task<IActionResult> AddService(ServiceFormViewModel viewModel, IFormFile ImageFile)
{
    var input = viewModel.Input;
    if (!ModelState.IsValid)
    {
        viewModel.Categories = _serviceService.GetAllCategories();
        return View("~/Views/Admin/ManageService/AddService.cshtml", viewModel);
    }

    string? imageUrl = null;
    if (ImageFile != null && ImageFile.Length > 0)
    {
        var account = new CloudinaryDotNet.Account(
            "dprp1jbd9", // cloud_name
            "584135338254938", // api_key
            "QbUYngPIdZcXEn_mipYn8RE5dlo" // api_secret
        );
        var cloudinary = new CloudinaryDotNet.Cloudinary(account);
        var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
        {
            File = new FileDescription(ImageFile.FileName, ImageFile.OpenReadStream()),
            Folder = "pet-spa/services"
        };
        var uploadResult = await cloudinary.UploadAsync(uploadParams);
        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        {
            imageUrl = uploadResult.SecureUrl.ToString();
        }
    }

    // Map sang entity Service
    var service = new Service
    {
        Name = input.Name?.Trim() ?? string.Empty,
        Price = input.Price ?? 0,
        CategoryId = input.CategoryId ?? 0,
        Description = input.Description?.Trim() ?? string.Empty,
        DurationMinutes = input.DurationMinutes ?? 0,
        IsActive = input.IsActive ?? true,
        CreatedAt = input.CreatedAt ?? DateTime.Now,
        ImageUrl = imageUrl
    };

    _serviceService.AddService(service);
    _serviceService.Save();
    TempData["SuccessMessage"] = "Thêm dịch vụ thành công!";
    return RedirectToAction("ServiceList");
}



            // ===== SOFT DELETE SERVICE =====
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult SoftDeleteService(int serviceId)
            {
                try
                {
                    var service = _serviceService.GetServiceById(serviceId);
                    if (service == null)
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy dịch vụ để tạm ngưng.";
                        return RedirectToAction("ServiceList");
                    }
                    service.IsActive = false;
                    _serviceService.UpdateService(service);
                    _serviceService.Save();
                    TempData["SuccessMessage"] = "Đã tạm ngưng dịch vụ thành công!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi khi tạm ngưng dịch vụ: " + ex.Message;
                }
                return RedirectToAction("ServiceList");
            }

        // ===== RESTORE SERVICE =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RestoreService(int serviceId)
        {
            try
            {
                var service = _serviceService.GetServiceById(serviceId);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy dịch vụ để kích hoạt lại.";
                    return RedirectToAction("ServiceList");
                }
                service.IsActive = true;
                _serviceService.UpdateService(service);
                _serviceService.Save();
                TempData["SuccessMessage"] = "Đã kích hoạt lại dịch vụ thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi kích hoạt lại dịch vụ: " + ex.Message;
            }
            return RedirectToAction("ServiceList");
        }

        // ===== INDEX PAGE =====
        public IActionResult Index()
        {
            try
            {
                var dashboardData = _serviceService.GetServiceDashboardViewModel();
                return RedirectToAction("ServiceDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang index");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải trang.";
                return View("~/Views/Admin/ManageService/Index.cshtml", new ServiceDashboardViewModel());
            }
        }

        // ===== SERVICE CATEGORY =====
        public IActionResult ServiceCategory()
        {
            try
            {
                var viewModel = new ServiceCategoryViewModel
                {
                    Categories = _serviceService.GetAllCategories(),
                    ServiceCountsByCategory = _serviceService.GetServiceCountsByCategory()
                };
                
                return View("~/Views/Admin/ManageService/ServiceCategory.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang danh mục");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh mục.";
                
                var emptyModel = new ServiceCategoryViewModel();
                return View("~/Views/Admin/ManageService/ServiceCategory.cshtml", emptyModel);
            }
        }

        // Các action category khác...
        [HttpPost]
        public IActionResult AddServiceCategory(SerCate category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _serviceService.AddCategory(category);
                    TempData["SuccessMessage"] = "Đã thêm danh mục dịch vụ thành công!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi khi thêm danh mục: " + ex.Message;
                }
            }
            
            return RedirectToAction("ServiceCategory");
        }

         [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExportToExcel()
        {
            var allServices = _serviceService.GetAllServicesViewModel();
            if (allServices == null || !allServices.Any())
            {
                TempData["ErrorMessage"] = "Không có dữ liệu để xuất.";
                return RedirectToAction("ServiceList");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Danh Sach Dich Vu");
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Tên dịch vụ";
                worksheet.Cell(1, 3).Value = "Danh mục";
                worksheet.Cell(1, 4).Value = "Giá";
                worksheet.Cell(1, 5).Value = "Trạng thái";

                int row = 2;
                foreach (var s in allServices)
                {
                    worksheet.Cell(row, 1).Value = $"SV-{s.ServiceId:D4}";
                    worksheet.Cell(row, 2).Value = s.Name;
                    worksheet.Cell(row, 3).Value = s.CategoryName;
                    worksheet.Cell(row, 4).Value = s.Price;
                    worksheet.Cell(row, 5).Value = (s.IsActive == true) ? "Hoạt động" : "Tạm ngưng";
                    row++;
                }
                worksheet.Columns().AdjustToContents();

                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    var fileName = $"DanhSachDichVu_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }


        // Helper class
        public class CategoryOrderItem
        {
            public int CategoryId { get; set; }
            public int DisplayOrder { get; set; }
        }
    }
}
