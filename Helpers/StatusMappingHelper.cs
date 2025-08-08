using System;
using System.Collections.Generic;
using System.Linq;

namespace pet_spa_system1.Helpers
{
    /// <summary>
    /// Helper class để mapping giữa AppointmentServiceStatus và Status_Appointment
    /// Giải quyết vấn đề 2 hệ thống status khác nhau
    /// </summary>
    public static class StatusMappingHelper
    {
        // =====================================================
        // MAPPING CONSTANTS
        // =====================================================
        
        /// <summary>
        /// AppointmentServiceStatus (1-4)
        /// </summary>
        public static class ServiceStatus
        {
            public const int Pending = 1;
            public const int InProgress = 2;
            public const int Completed = 3;
            public const int Cancelled = 4;
        }

        /// <summary>
        /// Status_Appointment (1-6)
        /// </summary>
        public static class AppointmentStatus
        {
            public const int Pending = 1;
            public const int Confirmed = 2;
            public const int InProgress = 3;
            public const int Completed = 4;
            public const int Cancelled = 5;
            public const int PendingCancel = 6;
        }

        // =====================================================
        // MAPPING METHODS
        // =====================================================

        /// <summary>
        /// Convert từ AppointmentServiceStatus sang Status_Appointment
        /// </summary>
        /// <param name="serviceStatus">Service status (1-4)</param>
        /// <returns>Appointment status (1-6)</returns>
        public static int ServiceStatusToAppointmentStatus(int? serviceStatus)
        {
            return serviceStatus switch
            {
                ServiceStatus.Pending => AppointmentStatus.Pending,        // 1 -> 1
                ServiceStatus.InProgress => AppointmentStatus.InProgress,  // 2 -> 3
                ServiceStatus.Completed => AppointmentStatus.Completed,    // 3 -> 4
                ServiceStatus.Cancelled => AppointmentStatus.Cancelled,    // 4 -> 5
                null => AppointmentStatus.Pending,                         // null -> 1 (default)
                _ => AppointmentStatus.Pending                              // unknown -> 1 (default)
            };
        }

        /// <summary>
        /// Convert từ Status_Appointment sang AppointmentServiceStatus
        /// </summary>
        /// <param name="appointmentStatus">Appointment status (1-6)</param>
        /// <returns>Service status (1-4)</returns>
        public static int AppointmentStatusToServiceStatus(int appointmentStatus)
        {
            return appointmentStatus switch
            {
                AppointmentStatus.Pending => ServiceStatus.Pending,           // 1 -> 1
                AppointmentStatus.Confirmed => ServiceStatus.Pending,         // 2 -> 1 (confirmed nhưng service chưa bắt đầu)
                AppointmentStatus.InProgress => ServiceStatus.InProgress,     // 3 -> 2
                AppointmentStatus.Completed => ServiceStatus.Completed,       // 4 -> 3
                AppointmentStatus.Cancelled => ServiceStatus.Cancelled,       // 5 -> 4
                AppointmentStatus.PendingCancel => ServiceStatus.Pending,     // 6 -> 1 (chờ hủy nhưng chưa hủy)
                _ => ServiceStatus.Pending                                     // unknown -> 1 (default)
            };
        }

        // =====================================================
        // BUSINESS LOGIC METHODS
        // =====================================================

        /// <summary>
        /// Tính toán trạng thái appointment dựa trên trạng thái tất cả services
        /// </summary>
        /// <param name="serviceStatuses">Danh sách trạng thái của tất cả services</param>
        /// <returns>Trạng thái appointment phù hợp</returns>
        public static int CalculateAppointmentStatusFromServices(IEnumerable<int?> serviceStatuses)
        {
            var statuses = serviceStatuses.Where(s => s.HasValue).Select(s => s.Value).ToList();
            
            if (!statuses.Any())
            {
                return AppointmentStatus.Pending; // Không có service nào
            }

            // Kiểm tra các trường hợp
            bool allCompleted = statuses.All(s => s == ServiceStatus.Completed);
            bool allCancelled = statuses.All(s => s == ServiceStatus.Cancelled);
            bool anyInProgress = statuses.Any(s => s == ServiceStatus.InProgress);
            bool anyPending = statuses.Any(s => s == ServiceStatus.Pending);
            bool anyCompleted = statuses.Any(s => s == ServiceStatus.Completed);
            bool anyActive = statuses.Any(s => s != ServiceStatus.Cancelled); // Có service không bị hủy

            // Logic quyết định trạng thái appointment (theo thứ tự ưu tiên)
            if (allCancelled)
            {
                // Tất cả services đều bị hủy -> Appointment bị hủy
                return AppointmentStatus.Cancelled;
            }
            else if (anyInProgress)
            {
                // Có ít nhất 1 service đang thực hiện -> Appointment đang thực hiện
                // (Ưu tiên cao vì đang có hoạt động)
                return AppointmentStatus.InProgress;
            }
            else if (allCompleted)
            {
                // Tất cả services đều hoàn thành -> Appointment hoàn thành
                return AppointmentStatus.Completed;
            }
            else if (anyCompleted)
            {
                // Có ít nhất 1 service hoàn thành (mixed với cancelled/pending) -> Appointment hoàn thành
                // Lý do: Khách hàng đã nhận được dịch vụ, có thể thanh toán
                return AppointmentStatus.Completed;
            }
            else if (anyPending && anyActive)
            {
                // Có service đang chờ và không phải tất cả đều bị hủy -> Appointment đã xác nhận
                return AppointmentStatus.Confirmed;
            }
            else
            {
                // Trường hợp khác -> Pending
                return AppointmentStatus.Pending;
            }
        }

        // =====================================================
        // DISPLAY METHODS
        // =====================================================

        /// <summary>
        /// Lấy tên hiển thị của service status
        /// </summary>
        public static string GetServiceStatusName(int? status)
        {
            return status switch
            {
                ServiceStatus.Pending => "Chờ xử lý",
                ServiceStatus.InProgress => "Đang thực hiện",
                ServiceStatus.Completed => "Hoàn thành",
                ServiceStatus.Cancelled => "Đã hủy",
                null => "Chưa xác định",
                _ => "Không xác định"
            };
        }

        /// <summary>
        /// Lấy tên hiển thị của appointment status
        /// </summary>
        public static string GetAppointmentStatusName(int status)
        {
            return status switch
            {
                AppointmentStatus.Pending => "Chờ xác nhận",
                AppointmentStatus.Confirmed => "Đã xác nhận",
                AppointmentStatus.InProgress => "Đang thực hiện",
                AppointmentStatus.Completed => "Hoàn thành",
                AppointmentStatus.Cancelled => "Đã hủy",
                AppointmentStatus.PendingCancel => "Chờ hủy",
                _ => "Không xác định"
            };
        }

        /// <summary>
        /// Lấy CSS class cho service status
        /// </summary>
        public static string GetServiceStatusCssClass(int? status)
        {
            return status switch
            {
                ServiceStatus.Pending => "badge-warning",
                ServiceStatus.InProgress => "badge-info",
                ServiceStatus.Completed => "badge-success",
                ServiceStatus.Cancelled => "badge-danger",
                null => "badge-secondary",
                _ => "badge-secondary"
            };
        }

        /// <summary>
        /// Lấy CSS class cho appointment status
        /// </summary>
        public static string GetAppointmentStatusCssClass(int status)
        {
            return status switch
            {
                AppointmentStatus.Pending => "badge-warning",
                AppointmentStatus.Confirmed => "badge-primary",
                AppointmentStatus.InProgress => "badge-info",
                AppointmentStatus.Completed => "badge-success",
                AppointmentStatus.Cancelled => "badge-danger",
                AppointmentStatus.PendingCancel => "badge-warning",
                _ => "badge-secondary"
            };
        }

        // =====================================================
        // VALIDATION METHODS
        // =====================================================

        /// <summary>
        /// Kiểm tra service status có hợp lệ không
        /// </summary>
        public static bool IsValidServiceStatus(int? status)
        {
            return status.HasValue && status >= 1 && status <= 4;
        }

        /// <summary>
        /// Kiểm tra appointment status có hợp lệ không
        /// </summary>
        public static bool IsValidAppointmentStatus(int status)
        {
            return status >= 1 && status <= 6;
        }

        /// <summary>
        /// Kiểm tra có thể cập nhật service status không
        /// </summary>
        public static bool CanUpdateServiceStatus(int? currentStatus, int newStatus)
        {
            // Không thể cập nhật service đã hoàn thành hoặc đã hủy
            if (currentStatus == ServiceStatus.Completed || currentStatus == ServiceStatus.Cancelled)
            {
                return false;
            }

            // Kiểm tra new status có hợp lệ không
            return IsValidServiceStatus(newStatus);
        }
    }
}
