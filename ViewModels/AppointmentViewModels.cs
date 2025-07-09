using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.ViewModels
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
        public string StatusName { get; set; }
        public List<string> PetNames { get; set; }
        public string Notes { get; set; }
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
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        public int UserId { get; set; }
        
        public int? EmployeeId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
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
            public string CustomerName { get; set; }
            public DateTime AppointmentDate { get; set; }
            public string StatusName { get; set; }
            public int StatusId { get; set; }
            public string PetNames { get; set; }
            public string ServiceNames { get; set; }
        }
    }
    #endregion
} 