using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using pet_spa_system1.Helpers;

namespace pet_spa_system1.Helpers
{
    /// <summary>
    /// Helper methods cho Razor Views
    /// </summary>
    public static class ViewHelper
    {
        /// <summary>
        /// Tạo badge HTML cho service status
        /// </summary>
        public static IHtmlContent ServiceStatusBadge(int? status)
        {
            var statusName = StatusMappingHelper.GetServiceStatusName(status);
            var cssClass = StatusMappingHelper.GetServiceStatusCssClass(status);
            
            return new HtmlString($"<span class=\"badge {cssClass}\">{statusName}</span>");
        }

        /// <summary>
        /// Tạo badge HTML cho appointment status
        /// </summary>
        public static IHtmlContent AppointmentStatusBadge(int status)
        {
            var statusName = StatusMappingHelper.GetAppointmentStatusName(status);
            var cssClass = StatusMappingHelper.GetAppointmentStatusCssClass(status);
            
            return new HtmlString($"<span class=\"badge {cssClass}\">{statusName}</span>");
        }

        /// <summary>
        /// Lấy tên hiển thị cho service status (cho JavaScript)
        /// </summary>
        public static string GetServiceStatusDisplayName(int? status)
        {
            return StatusMappingHelper.GetServiceStatusName(status);
        }

        /// <summary>
        /// Lấy CSS class cho service status (cho JavaScript)
        /// </summary>
        public static string GetServiceStatusBootstrapClass(int? status)
        {
            var cssClass = StatusMappingHelper.GetServiceStatusCssClass(status);
            // Convert từ badge-* sang Bootstrap color class
            return cssClass.Replace("badge-", "");
        }

        /// <summary>
        /// Kiểm tra có thể edit service status không
        /// </summary>
        public static bool CanEditServiceStatus(int? status)
        {
            return status != StatusMappingHelper.ServiceStatus.Completed && 
                   status != StatusMappingHelper.ServiceStatus.Cancelled;
        }
    }
}
