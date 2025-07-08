using pet_spa_system1.Models;
using System;
using System.Collections.Generic;

namespace pet_spa_system1.ViewModels
{
    public class ServiceDetailViewModel
    {
        public Models.Service Service { get; set; } = new Models.Service();
        public string CategoryName { get; set; } = "Không có danh mục";
        public List<AppointmentServiceViewModel> AppointmentHistory { get; set; } = new List<AppointmentServiceViewModel>();
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public int CustomerCount { get; set; }
        public List<Models.Service> RelatedServices { get; set; } = new List<Models.Service>();
        public List<AppointmentListItemViewModel> UpcomingAppointments { get; set; } = new List<AppointmentListItemViewModel>();
        public List<ServiceChangeHistoryItem> ChangeHistory { get; set; } = new List<ServiceChangeHistoryItem>();
    }
}
