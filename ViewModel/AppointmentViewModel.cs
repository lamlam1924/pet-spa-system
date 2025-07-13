using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

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

        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Tr?ng thái l?ch h?n (dùng cho email)
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;

        // Lists for dropdowns
        public List<Service> Services { get; set; } = new();
        public List<SerCate> Categories { get; set; } = new();
        public List<Pet> Pets { get; set; } = new();

        // For email confirmation
        public List<Pet> SelectedPets { get; set; } = new();
        public List<Service> SelectedServices { get; set; } = new();
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
    public class AdminAppointmentViewModel : AppointmentBaseViewModel
    {
        [Required(ErrorMessage = "Vui lòng ch?n khách hàng")]
        public int UserId { get; set; }

        public int? EmployeeId { get; set; }

        [Required(ErrorMessage = "Vui lòng ch?n tr?ng thái")]
        public int StatusId { get; set; }

        // Lists for dropdowns and selections
        public List<Service> AllServices { get; set; } = new();
        public List<SerCate> Categories { get; set; } = new();
        public List<User> Customers { get; set; } = new();
        public List<User> Employees { get; set; } = new();
        public List<StatusAppointment> Statuses { get; set; } = new();
        public List<Pet> AllPets { get; set; } = new();

        // Additional properties for display
        public DateTime? LastUpdated { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? EmployeeName { get; set; }
        public string? StatusName { get; set; }
        public decimal TotalPrice { get; set; }

        // Helper properties
        public int CustomerId { get => UserId; set => UserId = value; }
    }

    public class AppointmentDashboardViewModel
    {
        // Overview statistics
        public int TodayAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }

        // Monthly statistics
        public List<MonthlyAppointmentStats> MonthlyStats { get; set; } = new();

        // Today's appointments
        public List<DailyAppointment> RecentAppointments { get; set; } = new();

        public class DailyAppointment
        {
            public int AppointmentId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public DateTime AppointmentDate { get; set; }
            public string StatusName { get; set; } = string.Empty;
            public int StatusId { get; set; }
            public string PetNames { get; set; } = string.Empty;
            public string ServiceNames { get; set; } = string.Empty;
        }
    }
    #endregion

    #region Admin Appointment Detail
    public class AdminAppointmentDetailViewModel
    {
        // L?ch h?n
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Notes { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;

        // Khách hàng
        public int UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }

        // Nhân viên (có th? null)
        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeePhone { get; set; }
        public string? EmployeeEmail { get; set; }

        // Thú c?ng
        public List<PetInfo> Pets { get; set; } = new();

        // D?ch v?
        public List<ServiceInfo> Services { get; set; } = new();

        // T?ng ti?n
        public decimal TotalPrice { get; set; }

        // Ngày t?o/c?p nh?t
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Khuy?n mãi (n?u có)
        public string? PromotionName { get; set; }
        public decimal? PromotionValue { get; set; }

        // Tr?ng thái màu (cho badge)
        public string? StatusColor { get; set; }
    }

    // Thông tin thú c?ng trong l?ch h?n
    public class PetInfo
    {
        public int PetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SpeciesName { get; set; }
        public string? Breed { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
    }

    // Thông tin d?ch v? trong l?ch h?n
    public class ServiceInfo
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public int? DurationMinutes { get; set; }
    }
    #endregion

    // ViewModel cho trang duy?t l?ch 2 tab (admin)
    public class ApprovalListTabsViewModel
    {
        public List<AdminAppointmentDetailViewModel> Pending { get; set; } = new();
        public List<AdminAppointmentDetailViewModel> PendingCancel { get; set; } = new();

        // Thêm 2 property ??m s? l??ng
        public int PendingCount => Pending?.Count ?? 0;
        public int PendingCancelCount => PendingCancel?.Count ?? 0;
    }

    public class AppointmentListViewModel
    {
        public List<Appointment> Appointments { get; set; } = new();
        public List<StatusAppointment> StatusList { get; set; } = new();
        // Thêm các property cho filter ho?c hi?n th? n?u c?n
        public List<User> Employees { get; set; } = new();
        public List<User> Customers { get; set; } = new();
        public List<Service> Services { get; set; } = new();
        public List<Pet> Pets { get; set; } = new();
    }
}