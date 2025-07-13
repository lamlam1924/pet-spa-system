using System.Collections.Generic;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Repositories
{
    public interface IAppointmentServiceRepository
    {
        // ===== CÁC METHODS CẦN THIẾT CHO BUSINESS LOGIC =====
        IEnumerable<AppointmentService> GetByServiceId(int serviceId);
        int GetBookingCountByServiceId(int serviceId);
        decimal GetRevenueByServiceId(int serviceId);
        bool ExistsByServiceId(int serviceId);

        // ===== CHỈ CẦN THIẾT CHO THỐNG KÊ TỔNG QUAN =====
        int GetTotalBookings();
        decimal GetTotalRevenue();

        void Save();
        
    }
}