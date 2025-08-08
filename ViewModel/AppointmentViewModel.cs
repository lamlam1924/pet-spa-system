using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel

{
    public class PetStaffAssignViewModel
    {
        public int PetId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public int? StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? OwnerName  { get; set; }
    }

    public class ServiceHistoryInfo
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public List<string> PetImages { get; set; } = new(); // Thêm property này
        public int StatusId { get; set; } // trạng thái từng dịch vụ
        public string StatusName { get; set; } = string.Empty; // tên trạng thái từng dịch vụ
        public List<string> PetImagesBefore { get; set; } = new();
        public List<string> PetImagesAfter { get; set; } = new();
    }

    public class AppointmentBaseViewModel
    {
        public int AppointmentId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        // For form binding: string property for StartTime (HH:mm)
        public string? StartTimeString { get; set; }
        public string? Notes { get; set; }
        public List<int> SelectedPetIds { get; set; } = new();
        public List<int> SelectedServiceIds { get; set; } = new();

        public void CalculateEndTime(List<AppointmentServiceInfo> services)
        {
            var totalMinutes = services?.Sum(s => s.DurationMinutes ?? 0) ?? 0;
            EndTime = StartTime.AddMinutes(totalMinutes);
        }
    }

    public class AppointmentViewModel : AppointmentBaseViewModel
    {
        // Các property bổ sung cho controller
        public int? NewHour { get; set; }
        public int? NewStaffId { get; set; }
        public DateTime? NewStart { get; set; }
        public int? NewEmployeeId { get; set; }

        // Đảm bảo property CustomerId để binding đúng với select2 và controller
        // --- Properties for user booking form (Views/Appointment/Appointment.cshtml) ---
        public List<Pet> Pets { get; set; } = new();
        public int? PetId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public List<Service> Services { get; set; } = new();

        public List<SerCate> Categories { get; set; } = new();

        // Đã có User ở trên, không khai báo lại
        public string? Phone { get; set; }

        // AppointmentDate, AppointmentTime, Notes đã có ở base
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? EmployeeName { get; set; }
        public List<int> EmployeeIds { get; set; } = new();
        public List<User> Customers { get; set; } = new();

        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Users { get; set; } =
            Enumerable.Empty<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Staffs { get; set; } =
            Enumerable.Empty<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

        public string CustomerName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public List<AppointmentPetViewModel> SelectedPets { get; set; } = new();
        public List<AppointmentServiceInfo> SelectedServices { get; set; } = new();
        public int? SuggestedStaffId { get; set; }
        public List<User> EmployeeList { get; set; } = new();
        public int StatusId { get; set; }
        public List<StatusAppointment> Statuses { get; set; } = new();
        public int CustomerId { get; set; }
        public int StaffId { get; set; } // Only one staff per appointment
        public List<PetInfo> AllPets { get; set; } = new();

        public List<Service> AllServices { get; set; } = new();

        // Bổ sung property cho 1 pet 1 staff
        public List<PetStaffAssignViewModel> PetStaffAssignments { get; set; } = new();

        // Bổ sung property để khớp với view (navigation/entity-like)
        public User? User { get; set; }
        public List<AppointmentPet>? AppointmentPets { get; set; }
        public List<AppointmentService>? AppointmentServices { get; set; }
        public bool? IsActive { get; set; }
        public StatusAppointment? Status { get; set; }
        public User? Employee { get; set; }

        // Property for safe DateTime formatting in view
        public DateTime AppointmentDateTime { get; set; }

        public class CategoryInfo
        {
            public int CategoryId { get; set; }
            public string? Name { get; set; }
            public List<ServiceInfo> Services { get; set; } = new();
        }
    }

    public class AppointmentPetViewModel
    {
        public int PetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public string? StaffName { get; set; } // Thêm property để hiển thị nhân viên phụ trách từng pet
        public int AppointmentId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int StatusId { get; set; }
        public int StaffId { get; set; }
    }

    public class AppointmentServiceInfo
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public int? DurationMinutes { get; set; }
    }

    public class AppointmentListViewModel
    {
        public List<AppointmentViewModel> Appointments { get; set; } = new();
        public List<StatusAppointment> StatusList { get; set; } = new();
        public List<User> EmployeeList { get; set; } = new();
        public List<User> Customers { get; set; } = new();
        public List<Service> Services { get; set; } = new();
        public List<Pet> Pets { get; set; } = new();

        // Bổ sung cho phân trang
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class AppointmentItemViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public List<ServiceViewModel> Services { get; set; } = new();
        // public int DurationMinutes { get; set; }
        // public DateTime EndTime { get; set; }
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
                AppointmentDate = appointment.AppointmentDate.ToDateTime(appointment.StartTime),
                CustomerName = appointment.User?.FullName ?? "",
                CustomerPhone = appointment.User?.Phone,
                StatusId = appointment.StatusId,
                StatusName = appointment.Status?.StatusName ?? "",
                StaffName = appointment.Employee?.FullName,
                PetNames = appointment.AppointmentPets?.Select(p => p.Pet.Name).ToList() ?? new List<string>(),
                Services = appointment.AppointmentServices?.Select(s => new ServiceViewModel
                {
                    Name = s.Service.Name,
                    Duration = s.Service.DurationMinutes ?? 0,
                    Price = s.Service.Price
                }).ToList() ?? new List<ServiceViewModel>()
            };
        }
    }

    public class AppointmentHistoryItemViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
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

    public class MonthlyAppointmentStatsViewModel
    {
        public string MonthLabel { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
    }

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


    public class AdminAppointmentDetailViewModel
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Notes { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int TotalDurationMinutes { get; set; }
        public int UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeePhone { get; set; }
        public string? EmployeeEmail { get; set; }
        public List<PetInfo> Pets { get; set; } = new();
        public List<ServiceInfo> Services { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? PromotionName { get; set; }
        public decimal? PromotionValue { get; set; }
        public string? StatusColor { get; set; }

        // Các property phục vụ chỉnh sửa
        public List<StatusAppointment> StatusList { get; set; } = new();
        public List<int> SelectedPetIds { get; set; } = new();
        public List<PetInfo> AllPets { get; set; } = new();
        public List<int> SelectedServiceIds { get; set; } = new();
        public List<ServiceInfo> AllServices { get; set; } = new();
        public List<int> EmployeeIds { get; set; } = new();
        public List<User> EmployeeList { get; set; } = new();
        public List<AppointmentPetViewModel> SelectedPets { get; set; } = new List<AppointmentPetViewModel>();
        public List<AppointmentServiceInfo> SelectedServices { get; set; } = new List<AppointmentServiceInfo>();
    }

    public class PetInfo
    {
        public int PetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SpeciesName { get; set; }
        public string? Breed { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
    }

    public class ServiceInfo
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public int? DurationMinutes { get; set; }
    }

    public class ApproveRequestModel
    {
        public int AppointmentId { get; set; }
        public int StaffId { get; set; }
    }

    public class AppointmentDashboardViewModel
    {
        public int TodayAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int PendingApprovalAppointments { get; set; }
        public int PendingCancelAppointments { get; set; }
        public List<AppointmentDashboardItemViewModel> RecentAppointments { get; set; } = new();
        public List<AppointmentDashboardMonthStat> MonthlyStats { get; set; } = new();
    }

    public class AppointmentDashboardItemViewModel
    {
        public int AppointmentId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string PetNames { get; set; } = string.Empty;
        public string ServiceNames { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }

    public class AppointmentDashboardMonthStat
    {
        public int Month { get; set; }
        public string MonthLabel { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
    }
}