using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class StaffDashboardViewModel
    {
        public User Staff { get; set; }
        public List<Appointment> TodayAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> ThisWeekAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> ThisMonthAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
        
        // Thống kê số lượng
        public int TodayCount { get; set; }
        public int WeekCount { get; set; }
        public int MonthCount { get; set; }
        public int UnreadNotificationCount { get; set; }
        
        // Thống kê hiệu suất
        public int CompletedAppointmentsThisMonth { get; set; }
        public int CancelledAppointmentsThisMonth { get; set; }
        public decimal CompletionRate { get; set; }
        public int UniqueCustomersThisMonth { get; set; }
        
        // Lịch hẹn tiếp theo
        public Appointment NextAppointment { get; set; }
        
        // Trạng thái hiện tại
        public bool IsCurrentlyBusy { get; set; }
        public string CurrentStatus { get; set; } = "Rảnh";
        
        // Thống kê doanh thu (nếu cần)
        public decimal RevenueThisMonth { get; set; }
        
        // Đánh giá từ khách hàng
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
