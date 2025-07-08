using pet_spa_system1.Models;
using pet_spa_system1.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pet_spa_system1.Services
{
    public interface IServiceService
    {
        // Các phương thức cơ bản
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
        
        // Phương thức cho danh mục dịch vụ
        List<SerCate> GetAllCategories();
        Dictionary<int, int> GetServiceCountsByCategory();
        SerCate GetCategoryById(int categoryId);
        void AddCategory(SerCate category);
        void UpdateCategory(SerCate category);
        bool DeleteCategory(int id);
        bool CategoryHasServices(int categoryId);
        
        // Phương thức cho thống kê
        List<Models.AppointmentService> GetAppointmentServicesByServiceId(int serviceId);
        List<Service> GetRelatedServices(int categoryId, int currentServiceId, int count);
        List<Appointment> GetUpcomingAppointmentsByService(int serviceId, int count);
        Dictionary<int, string> GetTopServicesBooking(int count);
        Dictionary<int, string> GetCategoryDistribution(int count);
        List<Service> GetRecentServices(int count);
        List<Appointment> GetUpcomingAppointments(int count);
        
        // Phương thức tổng hợp để tránh nhiều kết nối DB cùng lúc
        ServiceDetailViewModel GetServiceDetailData(int serviceId);
        ServiceDashboardViewModel GetServiceDashboardData();
    }
}
