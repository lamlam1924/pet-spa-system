using pet_spa_system1.Models;
using System.Collections.Generic;

namespace pet_spa_system1.ViewModels
{
    public class ServiceDashboardViewModel
    {
        // Thống kê tổng quan
        public int TotalServices { get; set; }
        public int ActiveServices { get; set; }
        public int InactiveServices { get; set; }
        public int TotalCategories { get; set; }
        
        // Dữ liệu danh sách
        public List<Models.Service> RecentServices { get; set; } = new List<Models.Service>();
        public List<AppointmentListItemViewModel> UpcomingAppointments { get; set; } = new List<AppointmentListItemViewModel>();
        
        // Dữ liệu biểu đồ
        public Dictionary<int, string> TopServiceBooking { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> CategoryDistribution { get; set; } = new Dictionary<int, string>();
    }
}
