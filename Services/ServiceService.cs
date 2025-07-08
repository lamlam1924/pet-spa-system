using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pet_spa_system1.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        // Các phương thức cơ bản
        public void AddService(Service service)
        {
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
            
            // Ensure CreatedAt has a value
            if (!service.CreatedAt.HasValue)
            {
                service.CreatedAt = DateTime.Now;
            }
            
            _serviceRepository.AddService(service);
        }

        public List<Service> GetActiveServices()
        {
            return _serviceRepository.GetActiveServices();
        }

        public ServiceViewModel GetAllService()
        {
            return _serviceRepository.GetAllService();
        }

        public Service GetServiceById(int id)
        {
            return _serviceRepository.GetServiceById(id);
        }

        public void RestoreService(int id)
        {
            _serviceRepository.RestoreService(id);
        }

        public void Save()
        {
            _serviceRepository.Save();
        }

        public void SoftDeleteService(int id)
        {
            _serviceRepository.SoftDeleteService(id);
        }

        public void DeleteService(Service service)
        {
            _serviceRepository.DeleteService(service);
        }

        public void UpdateService(Service service)
        {
            _serviceRepository.UpdateService(service);
        }

        public List<Service> GetAll()
        {
            return _serviceRepository.GetAll();
        }

        // Phương thức cho danh mục
        public List<SerCate> GetAllCategories()
        {
            return _serviceRepository.GetAllCategories();
        }

        public Dictionary<int, int> GetServiceCountsByCategory()
        {
            return _serviceRepository.GetServiceCountsByCategory();
        }

        public SerCate GetCategoryById(int categoryId)
        {
            return _serviceRepository.GetCategoryById(categoryId);
        }

        public void AddCategory(SerCate category)
        {
            _serviceRepository.AddCategory(category);
            _serviceRepository.Save();
        }

        public void UpdateCategory(SerCate category)
        {
            _serviceRepository.UpdateCategory(category);
            _serviceRepository.Save();
        }

        public bool DeleteCategory(int id)
        {
            var result = _serviceRepository.DeleteCategory(id);
            if (result)
            {
                _serviceRepository.Save();
            }
            return result;
        }

        public bool CategoryHasServices(int categoryId)
        {
            return _serviceRepository.CategoryHasServices(categoryId);
        }

        // Phương thức cho thống kê và truy vấn phức tạp
        public List<Models.AppointmentService> GetAppointmentServicesByServiceId(int serviceId)
        {
            return _serviceRepository.GetAppointmentServicesByServiceId(serviceId);
        }

        public List<Service> GetRelatedServices(int categoryId, int currentServiceId, int count)
        {
            return _serviceRepository.GetRelatedServices(categoryId, currentServiceId, count);
        }

        public List<Appointment> GetUpcomingAppointmentsByService(int serviceId, int count)
        {
            return _serviceRepository.GetUpcomingAppointmentsByService(serviceId, count);
        }

        public Dictionary<int, string> GetTopServicesBooking(int count)
        {
            return _serviceRepository.GetTopServicesBooking(count);
        }

        public Dictionary<int, string> GetCategoryDistribution(int count)
        {
            return _serviceRepository.GetCategoryDistribution(count);
        }

        public List<Service> GetRecentServices(int count)
        {
            return _serviceRepository.GetRecentServices(count);
        }

        public List<Appointment> GetUpcomingAppointments(int count)
        {
            return _serviceRepository.GetUpcomingAppointments(count);
        }

        // Phương thức tổng hợp để tránh nhiều kết nối DB cùng lúc
        public ServiceDetailViewModel GetServiceDetailData(int serviceId)
        {
            // Sử dụng phương thức mới từ repository
            return _serviceRepository.GetServiceDetailViewModel(serviceId);
        }

        // Phương thức tổng hợp cho trang tổng quan (dashboard)
        public ServiceDashboardViewModel GetServiceDashboardData()
        {
            // Sử dụng phương thức mới từ repository
            return _serviceRepository.GetServiceDashboardViewModel();
        }
    }
}