using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ISerCateRepository _categoryRepository;
        private readonly IAppointmentServiceRepository _appointmentServiceRepository;

        public ServiceService(
            IServiceRepository serviceRepository,
            ISerCateRepository categoryRepository,
            IAppointmentServiceRepository appointmentServiceRepository)
        {
            _serviceRepository = serviceRepository;
            _categoryRepository = categoryRepository;
            _appointmentServiceRepository = appointmentServiceRepository;
        }

        // ===== VIEWMODEL METHODS =====

        public ServiceListViewModel GetServiceListViewModel(ServiceFilterModel filter, int page = 1)
        {
            try
            {
                var pageSize = 10;
                var allServices = _serviceRepository.GetAll();
                var categories = _categoryRepository.GetAll();

                // Áp dụng filter
                var filteredServices = ApplyFilters(allServices, filter);

                // Áp dụng sorting
                var sortedServices = ApplySorting(filteredServices, filter);

                // Tính toán phân trang
                var totalItems = sortedServices.Count();
                var pagedServices = sortedServices.Skip((page - 1) * pageSize).Take(pageSize);

                // Tạo ServiceListItem
                var serviceListItems = pagedServices.Select(service => new ServiceListItem
                {
                    ServiceId = service.ServiceId,
                    Name = service.Name ?? "",
                    CategoryName = GetCategoryName(service.CategoryId),
                    Price = service.Price,
                    DurationMinutes = service.DurationMinutes,
                    IsActive = service.IsActive,
                    BookingCount = _appointmentServiceRepository.GetBookingCountByServiceId(service.ServiceId),
                    Revenue = _appointmentServiceRepository.GetRevenueByServiceId(service.ServiceId),
                    CreatedAt = service.CreatedAt,
                    Description = service.Description
                });

                return new ServiceListViewModel
                {
                    Services = serviceListItems,
                    Categories = categories,
                    Filter = filter ?? new ServiceFilterModel(),
                    Pagination = new PaginationModel
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalItems
                    },
                    Summary = new ServiceSummaryStats
                    {
                        TotalServices = allServices.Count(),
                        ActiveServices = allServices.Count(s => s.IsActive == true),
                        InactiveServices = allServices.Count(s => s.IsActive != true),
                        TotalRevenue = _appointmentServiceRepository.GetTotalRevenue(),
                        TotalBookings = _appointmentServiceRepository.GetTotalBookings()
                    }
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi khi tải danh sách dịch vụ: " + ex.Message, ex);
            }
        }

        public ServiceDashboardViewModel GetServiceDashboardViewModel()
        {
            try
            {
                var allServices = _serviceRepository.GetAll();
                var categories = _categoryRepository.GetAll();

                var totalServices = allServices.Count();
                var activeServices = allServices.Count(s => s.IsActive == true);
                var inactiveServices = totalServices - activeServices;
                var totalCategories = categories.Count();

                var totalBookings = _appointmentServiceRepository.GetTotalBookings();
                var totalRevenue = _appointmentServiceRepository.GetTotalRevenue();

                // Lấy dịch vụ mới nhất
                var recentServices = _serviceRepository.GetRecentServices(5);

                // Top dịch vụ nổi bật
                var topServices = allServices
                    .Select(s => new TopServiceItem
                    {
                        ServiceId = s.ServiceId,
                        ServiceName = s.Name ?? "",
                        CategoryName = GetCategoryName(s.CategoryId),
                        BookingCount = _appointmentServiceRepository.GetBookingCountByServiceId(s.ServiceId),
                        Revenue = _appointmentServiceRepository.GetRevenueByServiceId(s.ServiceId),
                        Price = s.Price,
                        IsActive = s.IsActive == true
                    })
                    .OrderByDescending(s => s.BookingCount)
                    .Take(5)
                    .ToList();

                // Thống kê theo danh mục (nếu cần)
                var categoryStats = categories.Select(c => new CategoryStatsItem
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.Name ?? "",
                    ServiceCount = allServices.Count(s => s.CategoryId == c.CategoryId),
                    BookingCount = allServices.Where(s => s.CategoryId == c.CategoryId)
                                           .Sum(s => _appointmentServiceRepository.GetBookingCountByServiceId(s.ServiceId)),
                    Revenue = allServices.Where(s => s.CategoryId == c.CategoryId)
                                        .Sum(s => _appointmentServiceRepository.GetRevenueByServiceId(s.ServiceId)),
                    Percentage = totalServices > 0 ?
                        (decimal)allServices.Count(s => s.CategoryId == c.CategoryId) / totalServices * 100 : 0
                }).ToList();

                return new ServiceDashboardViewModel
                {
                    TotalServices = totalServices,
                    ActiveServices = activeServices,
                    InactiveServices = inactiveServices,
                    TotalCategories = totalCategories,
                    TotalBookings = totalBookings,
                    TotalRevenue = totalRevenue,
                    RecentServices = recentServices,
                    TopServices = topServices,
                    CategoryStats = categoryStats
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi khi tải dashboard: " + ex.Message, ex);
            }
        }

        public ServiceDetailViewModel GetServiceDetailViewModel(int serviceId)
        {
            try
            {
                var service = _serviceRepository.GetServiceById(serviceId);
                if (service == null)
                {
                    return new ServiceDetailViewModel
                    {
                        Service = new Service(),
                        CategoryName = "Không tìm thấy",
                        AppointmentHistory = new List<AppointmentHistoryItem>(),
                        BookingCount = 0,
                        Revenue = 0,
                        CustomerCount = 0,
                        RelatedServices = new List<ServiceListItem>()
                    };
                }

                // Lấy các dịch vụ liên quan cùng danh mục, loại trừ chính nó
                var relatedServices = _serviceRepository.GetAll()
                    .Where(s => s.CategoryId == service.CategoryId && s.ServiceId != serviceId && s.IsActive == true)
                    .Take(6)
                    .ToList();

                var relatedServiceList = relatedServices.Select(s => new ServiceListItem
                {
                    ServiceId = s.ServiceId,
                    Name = s.Name,
                    CategoryName = GetCategoryName(s.CategoryId),
                    Price = s.Price,
                    DurationMinutes = s.DurationMinutes,
                    IsActive = s.IsActive,
                    Description = s.Description
                }).ToList();

                // Lấy lịch sử đặt lịch cho dịch vụ này
                var appointmentServices = _appointmentServiceRepository.GetByServiceId(serviceId);
                var appointmentHistory = appointmentServices
                    .Where(a => a.Appointment != null && a.Appointment.User != null && a.Appointment.Status != null)
                    .Select(a => new AppointmentHistoryItem
                    {
                        AppointmentId = a.AppointmentId,
                        CustomerName = a.Appointment.User.FullName,
                        AppointmentDate = a.Appointment.AppointmentDate,
                        StatusName = a.Appointment.Status.StatusName,
                        Price = a.Service?.Price ?? 0
                    })
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToList();

                return new ServiceDetailViewModel
                {
                    Service = service,
                    CategoryName = GetCategoryName(service.CategoryId),
                    AppointmentHistory = appointmentHistory,
                    BookingCount = appointmentHistory.Count,
                    Revenue = appointmentHistory.Sum(a => a.Price),
                    CustomerCount = appointmentHistory.Select(a => a.CustomerName).Distinct().Count(),
                    RelatedServices = relatedServiceList
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi tải chi tiết dịch vụ {serviceId}: " + ex.Message, ex);
            }
        }

        // ===== SERVICE OPERATIONS =====

        public Service? GetServiceById(int id) => _serviceRepository.GetServiceById(id);
        public IEnumerable<Service> GetAll() => _serviceRepository.GetAll();
        public IEnumerable<Service> GetActiveServices() => _serviceRepository.GetActiveServices(); // Thêm lại method này

        public void AddService(Service service)
        {
            ValidateService(service);
            service.IsActive ??= true;
            service.DurationMinutes ??= 30;
            service.CreatedAt ??= DateTime.Now;
            _serviceRepository.AddService(service);
        }

        public void UpdateService(Service service)
        {
            ValidateService(service);
            _serviceRepository.UpdateService(service);
        }

        public void DeleteService(Service service)
        {
            if (_appointmentServiceRepository.ExistsByServiceId(service.ServiceId))
                throw new InvalidOperationException("Không thể xóa dịch vụ đã có lịch hẹn");
            _serviceRepository.DeleteService(service);
        }

        public void SoftDeleteService(int id) => _serviceRepository.SoftDeleteService(id);
        public void RestoreService(int id) => _serviceRepository.RestoreService(id);

        // ===== CATEGORY OPERATIONS =====

        public IEnumerable<SerCate> GetAllCategories() => _categoryRepository.GetAll();

        public Dictionary<int, int> GetServiceCountsByCategory() => _categoryRepository.GetServiceCountsByCategory();

        public void AddCategory(SerCate category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Tên danh mục không được để trống");
            _categoryRepository.Add(category);
        }

        public void UpdateCategory(SerCate category) => _categoryRepository.Update(category);

        public bool DeleteCategory(int id)
        {
            if (_categoryRepository.HasServices(id))
                throw new InvalidOperationException("Không thể xóa danh mục đang có dịch vụ");
            return _categoryRepository.DeleteById(id);
        }

        // ===== APPOINTMENT SERVICE RELATIONS =====

        public IEnumerable<Models.AppointmentService> GetAppointmentServicesByServiceId(int serviceId)
            => _appointmentServiceRepository.GetByServiceId(serviceId);

        public void Save()
        {
            _serviceRepository.Save();
            _categoryRepository.Save();
            _appointmentServiceRepository.Save();
        }

        // ===== PRIVATE HELPERS =====

        private string GetCategoryName(int categoryId)
        {
            return _categoryRepository.GetById(categoryId)?.Name ?? "Chưa phân loại";
        }

        private static void ValidateService(Service service)
        {
            if (string.IsNullOrWhiteSpace(service.Name))
                throw new ArgumentException("Tên dịch vụ không được để trống");
            if (service.Price <= 0)
                throw new ArgumentException("Giá dịch vụ phải lớn hơn 0");
        }

        private static IEnumerable<Service> ApplyFilters(IEnumerable<Service> services, ServiceFilterModel? filter)
        {
            if (filter == null) return services;

            var query = services.AsQueryable();

            if (filter.CategoryId.HasValue)
                query = query.Where(s => s.CategoryId == filter.CategoryId.Value);

            if (!string.IsNullOrEmpty(filter.Search))
            {
                var keyword = filter.Search.Trim();
                query = query.Where(s =>
                    (!string.IsNullOrEmpty(s.Name) && s.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Description) && s.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    s.ServiceId.ToString().Contains(keyword)
                );
            }

            if (filter.PriceFrom.HasValue)
                query = query.Where(s => s.Price >= filter.PriceFrom.Value);

            if (filter.PriceTo.HasValue)
                query = query.Where(s => s.Price <= filter.PriceTo.Value);

            if (!string.IsNullOrEmpty(filter.Status))
            {
                if (filter.Status == "active")
                    query = query.Where(s => s.IsActive == true);
                else if (filter.Status == "inactive")
                    query = query.Where(s => s.IsActive != true);
            }

            if (filter.CreatedFrom.HasValue)
                query = query.Where(s => s.CreatedAt >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                query = query.Where(s => s.CreatedAt <= filter.CreatedTo.Value);

            return query;
        }

        private static IEnumerable<Service> ApplySorting(IEnumerable<Service> services, ServiceFilterModel filter)
        {
            switch (filter.Sort)
            {
                case "name_desc":
                    return services.OrderByDescending(s => s.Name ?? "");
                case "price_asc":
                    return services.OrderBy(s => s.Price);
                case "price_desc":
                    return services.OrderByDescending(s => s.Price);
                case "date_asc":
                    return services.OrderBy(s => s.CreatedAt ?? DateTime.MinValue);
                case "date_desc":
                    return services.OrderByDescending(s => s.CreatedAt ?? DateTime.MinValue);
                default:
                    return services.OrderBy(s => s.Name ?? "");
            }
        }
        
        // Lấy toàn bộ danh sách dịch vụ cho xuất Excel (không phân trang)
        public List<ServiceListItem> GetAllServicesViewModel()
        {
            var allServices = _serviceRepository.GetAll();
            var categories = _categoryRepository.GetAll();
            var result = (from s in allServices
                          join c in categories on s.CategoryId equals c.CategoryId into sc
                          from c in sc.DefaultIfEmpty()
                          select new ServiceListItem
                          {
                              ServiceId = s.ServiceId,
                              Name = s.Name,
                              Description = s.Description,
                              Price = s.Price,
                              IsActive = s.IsActive,
                              CategoryName = c != null ? c.Name : null
                          }).ToList();
            return result;
        }
    }
}
