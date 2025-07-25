using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class ServiceHistoryInfo
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Duration { get; set; }
    }

    #region Base Models

    public class AppointmentBaseViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string? Notes { get; set; }
        public List<int> SelectedPetIds { get; set; } = new();
        public List<int> SelectedServiceIds { get; set; } = new();
    }

    #endregion


    #region Customer Appointment
    public class AppointmentViewModel : AppointmentBaseViewModel
    {
        // Danh sách nhân viên cho UI duyệt/gán
        public List<User> EmployeeList { get; set; } = new();
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Thông tin user hiện tại (nếu đã đăng nhập)
        public User? User { get; set; }

        // Trạng thái lịch hẹn (dùng cho email)
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;

        // Lists for dropdowns
        public List<Service> Services { get; set; } = new();
        public List<SerCate> Categories { get; set; } = new();
        public List<Pet> Pets { get; set; } = new();

        // For email confirmation
        public List<Pet> SelectedPets { get; set; } = new();
        public List<Service> SelectedServices { get; set; } = new();

        // --- Gộp từ AdminAppointmentViewModel ---
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public List<int> EmployeeIds { get; set; } = new();
        public int CustomerId { get; set; }
        public string? Notes { get; set; }
        public List<StatusAppointment> Statuses { get; set; } = new();
        public List<User> Customers { get; set; } = new();
        public List<PetInfo> AllPets { get; set; } = new();
        public List<Service> AllServices { get; set; } = new();
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }
        public int? UserId { get; set; }
        public List<int> SelectedPetIds { get; set; } = new();
        public List<int> SelectedServiceIds { get; set; } = new();
        public List<CategoryInfo> CategoriesAdmin { get; set; } = new(); // Đổi tên để tránh trùng lặp
        public int DurationMinutes { get; set; }
        public List<AppointmentServiceViewModel> AppointmentServices { get; set; } = new();
        public DateTime EndTime { get; set; }
        public int? SuggestedStaffId { get; set; }
        // --- End gộp ---

        // Định nghĩa class CategoryInfo và ServiceInfo lồng bên trong ViewModel (nếu chưa có)
        public class CategoryInfo
        {
            public int CategoryId { get; set; }
            public string? Name { get; set; }
            public List<ServiceInfo> Services { get; set; } = new();
        }

        public class ServiceInfo
        {
            public int ServiceId { get; set; }
            public string? Name { get; set; }
            public decimal Price { get; set; }
            public int DurationMinutes { get; set; }
        }
    }

    public class AppointmentListViewModel
    {
        public List<Appointment> Appointments { get; set; } = new();

        public List<StatusAppointment> StatusList { get; set; } = new();

        // Danh sách nhân viên cho UI duyệt/gán
        public List<User> EmployeeList { get; set; } = new();

        public List<User> Customers { get; set; } = new();
        public List<Service> Services { get; set; } = new();
        public List<Pet> Pets { get; set; } = new();
    }

    public class AppointmentItemViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }

        // Dịch vụ
        public List<ServiceViewModel> Services { get; set; } = new();

        // Tổng thời gian (phút) => tính từ Service.Duration
        public int DurationMinutes { get; set; }

        // Giờ kết thúc => tính từ AppointmentDate + DurationMinutes
        public DateTime EndTime { get; set; }

        // Các phần khác (khách hàng, thú cưng, trạng thái, nhân viên,...)
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? StaffName { get; set; }
        public List<string> PetNames { get; set; } = new();
        public decimal TotalPrice => Services?.Sum(s => s.Price) ?? 0;
        
        public AppointmentItemViewModel MapToViewModel(Appointment appointment)
        {
            return new AppointmentItemViewModel
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                CustomerName = appointment.User?.FullName ?? "",
                CustomerPhone = appointment.User?.Phone,
                StatusId = appointment.StatusId,
                StatusName = appointment.Status?.StatusName ?? "",
                StaffName = appointment.Employee?.FullName,
                PetNames = appointment.AppointmentPets?.Select(p => p.Pet.Name).ToList() ?? new List<string>(),
                Services = appointment.AppointmentServices?.Select(s => new ServiceViewModel
                {
                    Name = s.Service.Name,
                    Duration = s.Service.DurationMinutes.Value,
                    Price = s.Service.Price
                }).ToList() ?? new List<ServiceViewModel>()
            };
        }

    }

    
    public class AppointmentHistoryItemViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public List<string> PetNames { get; set; } = new();
        public string Notes { get; set; } = string.Empty;
        public List<ServiceHistoryInfo> Services { get; set; } = new();
    }

    public class AppointmentHistoryViewModel
    {
        public List<AppointmentHistoryItemViewModel> Appointments { get; set; } = new();
        public List<StatusAppointment> Statuses { get; set; } = new();
    }

    #endregion

    #region Admin Appointment

    // ViewModel cho thống kê lịch hẹn theo tháng (dùng cho dashboard, báo cáo, v.v.)
    public class MonthlyAppointmentStatsViewModel
    {
        public string MonthLabel { get; set; } = string.Empty; // Ví dụ: "Tháng 1"
        public int AppointmentCount { get; set; }
    }

    // ViewModel cho danh sách lịch hẹn ngắn gọn (dùng cho dashboard, danh sách hôm nay, v.v.)
    public class DailyAppointmentSummaryViewModel
    {
        public int AppointmentId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string PetNames { get; set; } = string.Empty;
        public string ServiceNames { get; set; } = string.Empty;
    }

    public class AppointmentDashboardViewModel
    {
        // Overview statistics
        public int TodayAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int PendingApprovalAppointments { get; set; }
        public int PendingCancelAppointments { get; set; }

        // Monthly statistics
        public List<MonthlyAppointmentStatsViewModel> MonthlyStats { get; set; } = new();

        // Today's appointments
        public List<DailyAppointmentSummaryViewModel> RecentAppointments { get; set; } = new();
    }

    #endregion

    #region Admin Appointment Detail

    public class AdminAppointmentDetailViewModel
    {
        // Lịch hẹn
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Notes { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int TotalDurationMinutes { get; set; } // Tổng thời lượng dịch vụ

        // Khách hàng
        public int UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }

        // Nhân viên (có thể null)
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeePhone { get; set; }
        public string? EmployeeEmail { get; set; }

        // Thú cưng
        public List<PetInfo> Pets { get; set; } = new();

        // Dịch vụ
        public List<ServiceInfo> Services { get; set; } = new();

        // Tổng tiền
        public decimal TotalPrice { get; set; }

        // Ngày tạo/cập nhật
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Khuyến mãi (nếu có)
        public string? PromotionName { get; set; }
        public decimal? PromotionValue { get; set; }

        // Trạng thái màu (cho badge)
        public string? StatusColor { get; set; }
    }

    // Thông tin thú cưng trong lịch hẹn
    public class PetInfo
    {
        public int PetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SpeciesName { get; set; }
        public string? Breed { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
    }

    // Thông tin dịch vụ trong lịch hẹn
    public class ServiceInfo
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public int? DurationMinutes { get; set; }
    }

    #endregion

    public class ApproveRequestModel
    {
        public int AppointmentId { get; set; }
        public int StaffId { get; set; }
    }
}