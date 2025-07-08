using pet_spa_system1.Models;
using System.Collections.Generic;
using pet_spa_system1.ViewModels;
using System;

namespace pet_spa_system1.Repositories
{
    public interface IServiceRepository
    {
        // Thêm phương thức mới
        ServiceDetailViewModel GetServiceDetailViewModel(int serviceId);
        ServiceDashboardViewModel GetServiceDashboardViewModel();
        List<AppointmentServiceViewModel> GetAppointmentServiceViewModels(int serviceId);
        List<AppointmentListItemViewModel> GetAppointmentListItemViewModels(List<Appointment> appointments);
        
        // Các phương thức quản lý dịch vụ cơ bản
        ServiceViewModel GetAllService();
        Service GetServiceById(int id);
        void AddService(Service service);
        void UpdateService(Service service);
        void SoftDeleteService(int id);
        void RestoreService(int id);
        void DeleteService(Service service); // Xóa vĩnh viễn
        void Save();
        List<Service> GetActiveServices();
        List<Service> GetAll();
        
        // Các phương thức quản lý danh mục
        List<SerCate> GetAllCategories();
        Dictionary<int, int> GetServiceCountsByCategory();
        SerCate GetCategoryById(int categoryId);
        void AddCategory(SerCate category);
        void UpdateCategory(SerCate category);
        bool DeleteCategory(int id);
        bool CategoryHasServices(int categoryId);
        void UpdateCategoryDisplayOrder(int categoryId, int displayOrder);
        
        // Các phương thức cho thống kê và truy vấn phức tạp
        List<Models.AppointmentService> GetAppointmentServicesByServiceId(int serviceId);
        List<Service> GetRelatedServices(int categoryId, int currentServiceId, int count);
        List<Appointment> GetUpcomingAppointmentsByService(int serviceId, int count);
        Dictionary<int, string> GetTopServicesBooking(int count);
        Dictionary<int, string> GetCategoryDistribution(int count);
        List<Service> GetRecentServices(int count);
        List<Appointment> GetUpcomingAppointments(int count);
    }
}
