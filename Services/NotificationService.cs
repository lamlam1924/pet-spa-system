using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public NotificationService(INotificationRepository notificationRepository,
            IAppointmentRepository appointmentRepository)
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

        public void SendServiceCancelNotification(int appointmentId, int serviceId, string reason = "")
        {
            try
            {
                var appointment = _appointmentRepository.GetById(appointmentId);
                if (appointment == null)
                {
                    Console.WriteLine
                    ($"Không tìm thấy lịch hẹn {appointmentId} để gửi thông báo hủy dịch vụ",
                        appointmentId);
                    return;
                }

                var service = appointment.AppointmentServices
                    .FirstOrDefault(s => s.ServiceId == serviceId)?.Service;
                if (service == null)
                {
                    Console.WriteLine
                        ($"Không tìm thấy dịch vụ {serviceId} trong lịch hẹn {appointmentId}");
                    return;
                }

                var message =
                    $"Dịch vụ {service.Name} trong lịch hẹn #{appointmentId} của bạn đã tạm ngưng hoạt động." +
                    $"\nNgày hẹn: {appointment.AppointmentDate:dd/MM/yyyy}" +
                    $"\nGiờ bắt đầu: {appointment.StartTime:HH:mm}" +
                    (string.IsNullOrEmpty(reason) ? "" : $"\nLý do: {reason}") +
                    "\nVui lòng chọn dịch vụ khác hoặc hủy lịch hẹn.";

                SendAppointmentNotification(appointmentId, "Thông báo về dịch vụ tạm ngưng", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Lỗi: {ex.Message} khi gửi thông báo hủy dịch vụ cho lịch hẹn {appointmentId}, dịch vụ {serviceId}");
            }
        }

        public void SendAppointmentNotification(int appointmentId, string title, string message)
        {
            try
            {
                var appointment = _appointmentRepository.GetById(appointmentId);
                if (appointment == null)
                {
                    Console.WriteLine($"Không tìm thấy lịch hẹn {appointmentId} để gửi thông báo");
                    return;
                }

                var notification = new Notification
                {
                    UserId = appointment.UserId,
                    Title = title,
                    Message = message,
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                    // Type = "Appointment"
                };

                _notificationRepository.AddNotificationAsync(notification);
                Console.WriteLine($"Đã gửi thông báo cho lịch hẹn {appointmentId}: {title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message} khi gửi thông báo cho lịch hẹn {appointmentId}");
            }
        }

// Thông báo xác nhận lịch hẹn
        public void SendAppointmentConfirmNotification(int appointmentId)
        {
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null) return;

            var message = $"Lịch hẹn #{appointmentId} của bạn đã được xác nhận." +
                          $"\nNgày hẹn: {appointment.AppointmentDate:dd/MM/yyyy}" +
                          $"\nGiờ bắt đầu: {appointment.StartTime:HH:mm}";

            SendAppointmentNotification(appointmentId, "Xác nhận lịch hẹn", message);
        }

// Thông báo hủy lịch hẹn
        public void SendAppointmentCancelNotification(int appointmentId, string reason = "")
        {
            var appointment = _appointmentRepository.GetById(appointmentId);
            if (appointment == null) return;

            var message = $"Lịch hẹn #{appointmentId} của bạn đã bị hủy." +
                          $"\nNgày hẹn: {appointment.AppointmentDate:dd/MM/yyyy}" +
                          $"\nGiờ bắt đầu: {appointment.StartTime:HH:mm}" +
                          (string.IsNullOrEmpty(reason) ? "" : $"\nLý do: {reason}");

            SendAppointmentNotification(appointmentId, "Hủy lịch hẹn", message);
        }
    }
}