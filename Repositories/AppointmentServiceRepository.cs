using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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
            => _context.AppointmentServices
                .Include(a => a.Appointment)
                    .ThenInclude(ap => ap.User)
                .Include(a => a.Appointment)
                    .ThenInclude(ap => ap.Status)
                .Include(a => a.Service)
                .Where(a => a.ServiceId == serviceId)
                .ToList();

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

        // AppointmentServiceImage
        public IEnumerable<AppointmentServiceImage> GetImagesByAppointmentServiceId(int appointmentServiceId)
            => _context.AppointmentServiceImages.Where(i => i.AppointmentServiceId == appointmentServiceId).ToList();

        public AppointmentServiceImage? GetImageById(int imageId)
            => _context.AppointmentServiceImages.Find(imageId);

        public void AddImage(AppointmentServiceImage image)
        {
            _context.AppointmentServiceImages.Add(image);
            _context.SaveChanges();
        }

        public void DeleteImage(int imageId)
        {
            var img = _context.AppointmentServiceImages.Find(imageId);
            if (img != null)
            {
                _context.AppointmentServiceImages.Remove(img);
                _context.SaveChanges();
            }
        }

        // AppointmentServiceStatus
        public IEnumerable<AppointmentServiceStatus> GetAllStatuses()
            => _context.AppointmentServiceStatus.ToList();

        public AppointmentServiceStatus? GetStatusById(int statusId)
            => _context.AppointmentServiceStatus.Find(statusId);

        public void AddStatus(AppointmentServiceStatus status)
        {
            _context.AppointmentServiceStatus.Add(status);
            _context.SaveChanges();
        }

        public void DeleteStatus(int statusId)
        {
            var status = _context.AppointmentServiceStatus.Find(statusId);
            if (status != null)
            {
                _context.AppointmentServiceStatus.Remove(status);
                _context.SaveChanges();
            }
        }
    }
}