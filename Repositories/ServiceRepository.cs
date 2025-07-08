using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModels;
using pet_spa_system1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pet_spa_system1.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly PetDataShopContext _context;

        public ServiceRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public ServiceViewModel GetAllService()
        {
            return new ServiceViewModel
            {
                Services = _context.Services
                    .Include(s => s.Category)
                    .ToList(),
                Categories = _context.SerCates
                    .ToList()
            };
        }

        public Service GetServiceById(int id)
        {
            return _context.Services
                .Include(s => s.Category)
                .FirstOrDefault(s => s.ServiceId == id);
        }

        public void AddService(Service service)
        {
            _context.Services.Add(service);
        }

        public void UpdateService(Service service)
        {
            _context.Services.Update(service);
        }

        public void SoftDeleteService(int id)
        {
            var service = GetServiceById(id);
            if (service != null)
            {
                service.IsActive = false;
                UpdateService(service);
            }
        }

        public void RestoreService(int id)
        {
            var service = GetServiceById(id);
            if (service != null)
            {
                service.IsActive = true;
                UpdateService(service);
            }
        }
        
        public void DeleteService(Service service)
        {
            _context.Services.Remove(service);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public List<Service> GetActiveServices()
        {
            return _context.Services
                .Include(s => s.Category)
                .Where(s => s.IsActive==true && s.Category != null)
                .OrderBy(s => s.CategoryId)
                .ToList();
        }

        public List<Service> GetAll()
        {
            return _context.Services
                .Include(s => s.Category)
                .ToList();
        }
        
        // Các phương thức cho danh mục
        public List<SerCate> GetAllCategories()
        {
            return _context.SerCates.ToList();
        }

        public Dictionary<int, int> GetServiceCountsByCategory()
        {
            return _context.Services
                .GroupBy(s => s.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionary(x => x.CategoryId, x => x.Count);
        }

        public SerCate GetCategoryById(int categoryId)
        {
            return _context.SerCates.Find(categoryId);
        }

        public void AddCategory(SerCate category)
        {
            category.CreatedAt = DateTime.Now;
            _context.SerCates.Add(category);
        }

        public void UpdateCategory(SerCate category)
        {
            var existingCategory = _context.SerCates.Find(category.CategoryId);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                existingCategory.IsActive = category.IsActive;
                
                _context.SerCates.Update(existingCategory);
            }
        }

        public bool DeleteCategory(int id)
        {
            var category = _context.SerCates.Find(id);
            if (category == null)
            {
                return false;
            }
            
            if (CategoryHasServices(id))
            {
                return false;
            }
            
            _context.SerCates.Remove(category);
            return true;
        }

        public bool CategoryHasServices(int categoryId)
        {
            return _context.Services.Any(s => s.CategoryId == categoryId);
        }
        
        public void UpdateCategoryDisplayOrder(int categoryId, int displayOrder)
        {
            var category = _context.SerCates.Find(categoryId);
            if (category != null)
            {
                // Giả định có trường DisplayOrder trong SerCate
                // category.DisplayOrder = displayOrder;
                _context.SerCates.Update(category);
            }
        }

        // Các phương thức cho thống kê và truy vấn phức tạp
        public List<Models.AppointmentService> GetAppointmentServicesByServiceId(int serviceId)
        {
            return _context.AppointmentServices
                .AsNoTracking()
                .Include(a => a.Appointment)
                .ThenInclude(a => a.User)
                .Include(a => a.Appointment.Status)
                .Include(a => a.Service)
                .Where(a => a.ServiceId == serviceId)
                .OrderByDescending(a => a.Appointment.AppointmentDate)
                .ToList();
        }

        public List<Service> GetRelatedServices(int categoryId, int currentServiceId, int count)
        {
            return _context.Services
                .AsNoTracking()
                .Include(s => s.Category)
                .Where(s => s.CategoryId == categoryId && s.ServiceId != currentServiceId && s.IsActive == true)
                .OrderBy(s => Guid.NewGuid())
                .Take(count)
                .ToList();
        }

        public List<Appointment> GetUpcomingAppointmentsByService(int serviceId, int count)
        {
            return _context.Appointments
                .AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.Status)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(s => s.Service)
                .Where(a => a.AppointmentServices.Any(s => s.ServiceId == serviceId) && 
                          a.AppointmentDate > DateTime.Now &&
                          a.StatusId != 4 && // Không phải trạng thái đã hủy
                          a.IsActive == true)
                .OrderBy(a => a.AppointmentDate)
                .Take(count)
                .ToList();
        }

        public Dictionary<int, string> GetTopServicesBooking(int count)
        {
            var topServices = _context.AppointmentServices
                .GroupBy(a => a.ServiceId)
                .Select(g => new { ServiceId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .ToList();
                
            var serviceIds = topServices.Select(x => x.ServiceId).ToList();
            var serviceNames = _context.Services
                .Where(s => serviceIds.Contains(s.ServiceId))
                .ToDictionary(s => s.ServiceId, s => s.Name);
                
            var result = new Dictionary<int, string>();
            foreach (var item in topServices)
            {
                string name = "";
                if (serviceNames.TryGetValue(item.ServiceId, out string? serviceName) && serviceName != null)
                {
                    name = serviceName.Length > 15 ? serviceName.Substring(0, 15) + "..." : serviceName;
                }
                result[item.Count] = name;
            }
            
            return result;
        }

        public Dictionary<int, string> GetCategoryDistribution(int count)
        {
            var categoryDistribution = _context.Services
                .GroupBy(s => s.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .ToList();
                
            var categoryIds = categoryDistribution.Select(x => x.CategoryId).ToList();
            var categoryNames = _context.SerCates
                .Where(c => categoryIds.Contains(c.CategoryId))
                .ToDictionary(c => c.CategoryId, c => c.Name);
                
            var result = new Dictionary<int, string>();
            foreach (var item in categoryDistribution)
            {
                string name = "";
                if (categoryNames.TryGetValue(item.CategoryId, out string? categoryName) && categoryName != null)
                {
                    name = categoryName.Length > 15 ? categoryName.Substring(0, 15) + "..." : categoryName;
                }
                result[item.Count] = name;
            }
                
            return result;
        }

        public List<Service> GetRecentServices(int count)
        {
            return _context.Services
                .AsNoTracking()
                .Include(s => s.Category)
                .OrderByDescending(s => s.CreatedAt)
                .Take(count)
                .ToList();
        }

        public List<Appointment> GetUpcomingAppointments(int count)
        {
            return _context.Appointments
                .AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.AppointmentServices)
                .ThenInclude(a => a.Service)
                .Include(a => a.Status)
                .Where(a => a.AppointmentDate > DateTime.Now && 
                            a.StatusId != 4 && 
                            a.IsActive == true)
                .OrderBy(a => a.AppointmentDate)
                .Take(count)
                .ToList();
        }
        
        // Thêm phương thức để tổng hợp dữ liệu chi tiết dịch vụ
        public ServiceDetailViewModel GetServiceDetailData(int serviceId)
        {
            var service = GetServiceById(serviceId);
            if (service == null)
                return new ServiceDetailViewModel();
                
            int categoryId = 0;
            if (service.CategoryId > 0)  // Loại bỏ kiểm tra null vì int không thể null
            {
                categoryId = service.CategoryId;  // Gán trực tiếp không cần GetValueOrDefault
            }
            var category = GetCategoryById(categoryId);
            
            // Lấy danh sách lịch sử đặt lịch
            var appointmentServices = GetAppointmentServicesByServiceId(serviceId);
            
            // Chuyển đổi thành ViewModels
            var appointmentServiceViewModels = new List<AppointmentServiceViewModel>();
            foreach (var a in appointmentServices)
            {
                var viewModel = new AppointmentServiceViewModel
                {
                    AppointmentId = a.Appointment?.AppointmentId ?? 0,
                    AppointmentDate = a.Appointment?.AppointmentDate ?? DateTime.MinValue,
                    UserId = a.Appointment?.UserId ?? 0,
                    CustomerName = ModelUtils.GetUserFullName(a.Appointment?.User),
                    StatusId = a.Appointment?.StatusId ?? 0,
                    StatusName = ModelUtils.GetStatusName(a.Appointment?.Status),
                    ServiceId = a.Service?.ServiceId ?? 0,
                    ServiceName = a.Service?.Name ?? "Không xác định",
                    ServicePrice = a.Service?.Price ?? 0,
                    Price = ModelUtils.GetAppointmentServicePrice(a)
                };
                appointmentServiceViewModels.Add(viewModel);
            }
            
            // Lấy dịch vụ liên quan (cùng danh mục)
            var relatedServices = GetRelatedServices(categoryId, serviceId, 4);
            
            // Lấy lịch hẹn sắp tới sử dụng dịch vụ này
            var upcomingAppointments = GetUpcomingAppointmentsByService(serviceId, 5);
            
            // Chuyển đổi thành ViewModels
            var upcomingAppointmentViewModels = new List<AppointmentListItemViewModel>();
            foreach (var a in upcomingAppointments)
            {
                var viewModel = new AppointmentListItemViewModel
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    CustomerName = ModelUtils.GetUserFullName(a.User),
                    MainServiceName = a.AppointmentServices.FirstOrDefault()?.Service?.Name ?? "N/A",
                    AdditionalServicesCount = a.AppointmentServices.Count - 1,
                    StatusId = a.StatusId,
                    StatusName = ModelUtils.GetStatusName(a.Status)
                };
                upcomingAppointmentViewModels.Add(viewModel);
            }
            
            // Tính toán thống kê
            var bookingCount = appointmentServiceViewModels.Count;
            var revenue = appointmentServiceViewModels.Sum(a => a.Price);
            var customerCount = appointmentServiceViewModels.Select(a => a.UserId).Distinct().Count();
            
            // Thêm lịch sử thay đổi
            var changeHistory = GetServiceChangeHistory(service);
            
            return new ServiceDetailViewModel
            {
                Service = service,
                CategoryName = category?.Name ?? "Không có danh mục",
                AppointmentHistory = appointmentServiceViewModels,
                RelatedServices = relatedServices,
                UpcomingAppointments = upcomingAppointmentViewModels,
                BookingCount = bookingCount,
                Revenue = revenue,
                CustomerCount = customerCount,
                ChangeHistory = changeHistory
            };
        }
        
        // Thêm phương thức để tổng hợp dữ liệu dashboard
        public ServiceDashboardViewModel GetServiceDashboardData()
        {
            var allServices = GetAll();
            var activeServices = allServices.Where(s => s.IsActive == true).ToList();
            var inactiveServices = allServices.Where(s => s.IsActive != true).ToList();
            var categories = GetAllCategories();
            
            // Lấy dữ liệu thống kê
            var topServices = GetTopServicesBooking(5);
            var categoryDistribution = GetCategoryDistribution(6);
            var recentServices = GetRecentServices(10);
            
            // Lấy lịch hẹn sắp tới
            var upcomingAppointments = GetUpcomingAppointments(5);
            
            // Chuyển đổi thành ViewModels
            var upcomingAppointmentViewModels = upcomingAppointments.Select(a => new AppointmentListItemViewModel
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate,
                CustomerName = ModelUtils.GetUserFullName(a.User),
                MainServiceName = a.AppointmentServices.FirstOrDefault()?.Service?.Name ?? "N/A",
                AdditionalServicesCount = a.AppointmentServices.Count - 1,
                StatusId = a.StatusId,
                StatusName = ModelUtils.GetStatusName(a.Status)
            }).ToList();
            
            return new ServiceDashboardViewModel
            {
                TotalServices = allServices.Count,
                ActiveServices = activeServices.Count,
                InactiveServices = inactiveServices.Count,
                TotalCategories = categories.Count,
                RecentServices = recentServices,
                UpcomingAppointments = upcomingAppointmentViewModels,
                TopServiceBooking = topServices,
                CategoryDistribution = categoryDistribution
            };
        }

        // Thêm phương thức để triển khai interface
        public ServiceDashboardViewModel GetServiceDashboardViewModel()
        {
            // Gọi phương thức hiện có đã được triển khai
            return GetServiceDashboardData();
        }

        // Thêm các phương thức mới

        public List<AppointmentServiceViewModel> GetAppointmentServiceViewModels(int serviceId)
        {
            var appointmentServices = GetAppointmentServicesByServiceId(serviceId);
            
            return appointmentServices.Select(a => new AppointmentServiceViewModel
            {
                AppointmentId = a.Appointment?.AppointmentId ?? 0,
                AppointmentDate = a.Appointment?.AppointmentDate ?? DateTime.MinValue,
                UserId = a.Appointment?.UserId ?? 0,
                CustomerName = ModelUtils.GetUserFullName(a.Appointment?.User),
                StatusId = a.Appointment?.StatusId ?? 0,
                StatusName = ModelUtils.GetStatusName(a.Appointment?.Status),
                ServiceId = a.Service?.ServiceId ?? 0,
                ServiceName = a.Service?.Name ?? "Không xác định",
                ServicePrice = a.Service?.Price ?? 0,
                Price = ModelUtils.GetAppointmentServicePrice(a)
            }).ToList();
        }

        public List<AppointmentListItemViewModel> GetAppointmentListItemViewModels(List<Appointment> appointments)
        {
            if (appointments == null) return new List<AppointmentListItemViewModel>();
            
            return appointments.Select(a => new AppointmentListItemViewModel
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate,
                CustomerName = a.User?.FullName ?? "Không xác định",  // Thay đổi FirstName + LastName thành FullName
                MainServiceName = a.AppointmentServices.FirstOrDefault()?.Service?.Name ?? "N/A",
                AdditionalServicesCount = a.AppointmentServices.Count - 1,
                StatusId = a.StatusId,
                StatusName = a.Status?.StatusName ?? "N/A"  // Thay đổi Name thành StatusName
            }).ToList();
        }

        public ServiceDetailViewModel GetServiceDetailViewModel(int serviceId)
        {
            var service = GetServiceById(serviceId);
            if (service == null)
                return new ServiceDetailViewModel();
            
            int categoryId = 0;
            if (service.CategoryId > 0)  // Loại bỏ kiểm tra null vì int không thể null
            {
                categoryId = service.CategoryId;  // Gán trực tiếp không cần GetValueOrDefault
            }
            var category = GetCategoryById(categoryId);
            
            // Lấy danh sách lịch sử đặt lịch
            var appointmentServices = GetAppointmentServicesByServiceId(serviceId);
            
            // Chuyển đổi sang ViewModels
            var appointmentServiceViewModels = appointmentServices.Select(a => new AppointmentServiceViewModel
            {
                AppointmentId = a.Appointment?.AppointmentId ?? 0,
                AppointmentDate = a.Appointment?.AppointmentDate ?? DateTime.MinValue,
                UserId = a.Appointment?.UserId ?? 0,
                CustomerName = ModelUtils.GetUserFullName(a.Appointment?.User),
                StatusId = a.Appointment?.StatusId ?? 0,
                StatusName = ModelUtils.GetStatusName(a.Appointment?.Status),
                ServiceId = a.Service?.ServiceId ?? 0,
                ServiceName = a.Service?.Name ?? "Không xác định",
                ServicePrice = a.Service?.Price ?? 0,
                Price = ModelUtils.GetAppointmentServicePrice(a)
            }).ToList();
            
            // Lấy các dịch vụ liên quan và lịch hẹn sắp tới
            var relatedServices = GetRelatedServices(categoryId, serviceId, 4);
            var upcomingAppointments = GetUpcomingAppointmentsByService(serviceId, 5);
            
            // Chuyển đổi lịch hẹn sắp tới thành ViewModels
            var upcomingAppointmentViewModels = upcomingAppointments.Select(a => new AppointmentListItemViewModel
            {
                AppointmentId = a.AppointmentId,
                AppointmentDate = a.AppointmentDate,
                CustomerName = ModelUtils.GetUserFullName(a.User),
                MainServiceName = a.AppointmentServices.FirstOrDefault()?.Service?.Name ?? "N/A",
                AdditionalServicesCount = a.AppointmentServices.Count - 1,
                StatusId = a.StatusId,
                StatusName = ModelUtils.GetStatusName(a.Status)
            }).ToList();
            
            // Tính toán thống kê
            var bookingCount = appointmentServiceViewModels.Count;
            var revenue = appointmentServiceViewModels.Sum(a => a.Price);
            var customerCount = appointmentServiceViewModels.Select(a => a.UserId).Distinct().Count();
            
            // Lịch sử thay đổi
            var changeHistory = GetServiceChangeHistory(service);
            
            return new ServiceDetailViewModel
            {
                Service = service,
                CategoryName = category?.Name ?? "Không có danh mục",
                AppointmentHistory = appointmentServiceViewModels,
                RelatedServices = relatedServices,
                UpcomingAppointments = upcomingAppointmentViewModels,
                BookingCount = bookingCount,
                Revenue = revenue,
                CustomerCount = customerCount,
                ChangeHistory = changeHistory
            };
        }

        // Phương thức helper để lấy lịch sử thay đổi của service
        private List<ServiceChangeHistoryItem> GetServiceChangeHistory(Service service)
        {
            var result = new List<ServiceChangeHistoryItem>();
            if (service == null) return result;
            
            // Sử dụng reflection để tìm các thuộc tính liên quan đến thời gian
            var properties = typeof(Service).GetProperties();
            
            DateTime? modifiedDate = null;
            DateTime? createdDate = null;
            
            // Tìm thuộc tính ModifiedDate/UpdatedAt
            foreach (var prop in properties)
            {
                if (prop.Name.Contains("Modified") || prop.Name.Contains("Updated") || prop.Name.EndsWith("At"))
                {
                    var value = prop.GetValue(service);
                    if (value is DateTime dtValue)
                    {
                        modifiedDate = dtValue;
                        break;
                    }
                    // Sửa lỗi pattern matching
                    else if (value is DateTime?)
                    {
                        modifiedDate = (DateTime?)value;
                        break;
                    }
                }
            }
            
            // Tìm thuộc tính CreatedDate/CreatedAt
            foreach (var prop in properties)
            {
                if (prop.Name.Contains("Created") || prop.Name.StartsWith("Date"))
                {
                    var value = prop.GetValue(service);
                    if (value is DateTime dtValue)
                    {
                        createdDate = dtValue;
                        break;
                    }
                    // Sửa lỗi pattern matching
                    else if (value is DateTime?)
                    {
                        createdDate = (DateTime?)value;
                        break;
                    }
                }
            }
            
            // Thêm lịch sử nếu có ngày cập nhật
            if (modifiedDate.HasValue && createdDate.HasValue && modifiedDate > createdDate)
            {
                result.Add(ServiceChangeHistoryItem.Create(
                    modifiedDate.Value,
                    "Hệ thống",
                    "Cập nhật thông tin dịch vụ"
                ));
            }
            
            return result;
        }
    }
}
