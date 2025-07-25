using System;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public class AppointmentLogService : IAppointmentLogService
    {
        private readonly PetDataShopContext _context;
        public AppointmentLogService(PetDataShopContext context)
        {
            _context = context;
        }
        public void LogAction(int appointmentId, string action, string note, int? staffId = null)
        {
            // Không chỉnh sửa model, chỉ insert vào bảng AppointmentLog nếu đã có
            // Nếu chưa có bảng, chỉ log ra console
            Console.WriteLine($"[LOG] AppointmentId={appointmentId}, Action={action}, Note={note}, StaffId={staffId}");
            // Nếu có bảng AppointmentLog, có thể insert như sau:
            // _context.AppointmentLogs.Add(new AppointmentLog { ... });
            // _context.SaveChanges();
        }
    }
    public interface IAppointmentLogService
    {
        void LogAction(int appointmentId, string action, string note, int? staffId = null);
    }
}
