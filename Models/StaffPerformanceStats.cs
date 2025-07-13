using System;

namespace pet_spa_system1.Models
{
    public class StaffPerformanceStats
    {
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int UniqueCustomers { get; set; }
        public decimal TotalRevenue { get; set; }
        // Thêm các trường khác nếu cần
    }
} 