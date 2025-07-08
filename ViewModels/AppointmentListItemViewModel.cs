using System;
using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModels
{
    public class AppointmentListItemViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string MainServiceName { get; set; } = string.Empty;
        public int AdditionalServicesCount { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        
        // Factory method để tạo từ Model
        public static AppointmentListItemViewModel FromModel(Appointment appointment)
        {
            if (appointment == null) return new AppointmentListItemViewModel();
            
            var firstService = appointment.AppointmentServices?.FirstOrDefault()?.Service;
            
            return new AppointmentListItemViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                CustomerName = appointment.User?.FullName ?? "Không xác định",
                MainServiceName = firstService?.Name ?? "Không xác định",
                AdditionalServicesCount = (appointment.AppointmentServices?.Count ?? 1) - 1,
                StatusId = appointment.StatusId,
                StatusName = appointment.Status?.StatusName ?? "Không xác định"
            };
        }
    }
}
