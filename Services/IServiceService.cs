using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;
using pet_spa_system1.ViewModels;
using System.Collections.Generic;

namespace pet_spa_system1.Services
{
    public interface IServiceService
    {
        // ===== VIEWMODEL METHODS - CHÍNH =====
        ServiceListViewModel GetServiceListViewModel(ServiceFilterModel filter, int page = 1);
        ServiceDashboardViewModel GetServiceDashboardViewModel();
        ServiceDetailViewModel GetServiceDetailViewModel(int serviceId);

        // ===== SERVICE OPERATIONS =====
        Service? GetServiceById(int id);
        IEnumerable<Service> GetAll();
        IEnumerable<Service> GetActiveServices(); // Cần thiết cho controllers
        void AddService(Service service);
        void UpdateService(Service service);
        void DeleteService(Service service); // Cần thiết cho hard delete
        void SoftDeleteService(int id);
        void RestoreService(int id);

        // ===== CATEGORY OPERATIONS =====
        IEnumerable<SerCate> GetAllCategories();
        Dictionary<int, int> GetServiceCountsByCategory(); // Cần thiết cho controllers
        void AddCategory(SerCate category);
        void UpdateCategory(SerCate category);
        bool DeleteCategory(int id);

        // ===== APPOINTMENT SERVICE RELATIONS - ĐƠN GIẢN HÓA =====
        IEnumerable<Models.AppointmentService> GetAppointmentServicesByServiceId(int serviceId); // Cần thiết cho controllers

        void Save();

        // Lấy toàn bộ danh sách dịch vụ cho xuất Excel (không phân trang)
        List<ServiceListItem> GetAllServicesViewModel();
    }
}
