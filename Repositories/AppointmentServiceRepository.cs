using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModels;

namespace pet_spa_system1.Repositories
{
    public class AppointmentServiceRepository : IAppointmentServiceRepository
    {
        private readonly PetDataShopContext _context;

        public AppointmentServiceRepository(PetDataShopContext context)
        {
            _context = context;
        }

        // ===== BUSINESS LOGIC METHODS =====
        public IEnumerable<AppointmentService> GetByServiceId(int serviceId)
            => _context.AppointmentServices.Where(a => a.ServiceId == serviceId).ToList();

        public int GetBookingCountByServiceId(int serviceId)
            => _context.AppointmentServices.Count(a => a.ServiceId == serviceId);

        public decimal GetRevenueByServiceId(int serviceId)
        {
            // Tối ưu hơn: Join với bảng Services
            return _context.AppointmentServices
                .Where(a => a.ServiceId == serviceId)
                .Join(_context.Services,
                    a => a.ServiceId,
                    s => s.ServiceId,
                    (a, s) => s.Price)
                .Sum();
        }

        public bool ExistsByServiceId(int serviceId)
            => _context.AppointmentServices.Any(a => a.ServiceId == serviceId);

        // ===== THỐNG KÊ TỔNG QUAN =====
        public int GetTotalBookings() => _context.AppointmentServices.Count();

        public decimal GetTotalRevenue()
        {
            // Tối ưu hơn: Join với bảng Services thay vì call method riêng lẻ
            return _context.AppointmentServices
                .Join(_context.Services,
                    a => a.ServiceId,
                    s => s.ServiceId,
                    (a, s) => s.Price)
                .Sum();
        }

        public void Save() => _context.SaveChanges();
        
        // Thêm hàm chuyển đổi mã trạng thái sang tên
        private string GetStatusName(int? status)
        {
            return status switch
            {
                1 => "Chờ xác nhận",
                2 => "Đã xác nhận",
                3 => "Đang thực hiện",
                4 => "Hoàn thành",
                5 => "Đã hủy",
                _ => "Không xác định"
            };
        }
    }
}