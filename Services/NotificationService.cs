using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public NotificationService(INotificationRepository notificationRepository, IAppointmentRepository appointmentRepository)
        {
            _notificationRepository = notificationRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<Notification>> GetByUserIdAsync(int userId)
        {
            return await _notificationRepository.GetNotificationsByUserIdAsync(userId);
        }

        public async Task AddAsync(Notification notification)
        {
            await _notificationRepository.AddNotificationAsync(notification);
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        public async Task DeleteAllAsync(int userId)
        {
            await _notificationRepository.DeleteAllAsync(userId);
        }

        // public void SendServiceCancelNotification(int appointmentId, int serviceId, string reason = "")
        // {
        //     try
        //     {
        //         var appointment = _appointmentRepository.GetById(appointmentId);
        //         if (appointment == null) return;

        //         var service = appointment.AppointmentServices
        //             .FirstOrDefault(s => s.ServiceId == serviceId)?.Service;
        //         if (service == null) return;

        //         var notification = new Notification
        //         {
        //             UserId = appointment.UserId,
        //             Title = "Thông báo về dịch vụ tạm ngưng",
        //             Message = 
        //                 $"Dịch vụ {service.Name} trong lịch hẹn #{appointmentId} của bạn đã tạm ngưng hoạt động." +
        //                 $"\nNgày hẹn: {appointment.AppointmentDate:dd/MM/yyyy}" +
        //                 $"\nGiờ bắt đầu: {appointment.StartTime:HH:mm}" +
        //                 (string.IsNullOrEmpty(reason) ? "" : $"\nLý do: {reason}") +
        //                 "\nVui lòng chọn dịch vụ khác hoặc hủy lịch hẹn.",
        //             CreatedAt = DateTime.Now,
        //             IsRead = false,
        //         };

        //         _notificationRepository.AddNotificationAsync(notification);
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine ($"Lỗi khi gửi thông báo hủy dịch vụ: {ex.Message}");
               
        //     }
        // }
    }
}