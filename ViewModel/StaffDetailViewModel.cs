using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class StaffDetailViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public int TodayCount { get; set; }
        public int MonthCount { get; set; }
        public List<Appointment> RecentAppointments { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public List<Appointment> AllAppointments { get; set; }
        // Hiệu suất làm việc
        public StaffPerformanceStats? PerformanceStats { get; set; } // Thống kê hiệu suất
        // Đánh giá của khách hàng về nhân viên
        public List<Review>? Reviews { get; set; }
        // Tài liệu liên quan đến nhân viên
        public List<StaffDocument>? Documents { get; set; }
        // Kết quả reset mật khẩu (nếu có)
        public string? ResetPasswordResult { get; set; }
    }
} 