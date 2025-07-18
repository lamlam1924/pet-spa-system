using pet_spa_system1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace pet_spa_system1.ViewModel
{
    // ===== UNIFIED SERVICE VIEWMODELS =====

    // Service List
    public class ServiceListViewModel
    {
        public IEnumerable<ServiceListItem> Services { get; set; } = new List<ServiceListItem>();
        public IEnumerable<SerCate> Categories { get; set; } = new List<SerCate>();
        public ServiceFilterModel Filter { get; set; } = new();
        public PaginationModel Pagination { get; set; } = new();
        public ServiceSummaryStats Summary { get; set; } = new();
    }

    public class ServiceListItem
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public decimal Price { get; set; }
        public int? DurationMinutes { get; set; }
        public bool? IsActive { get; set; }
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public DateTime? CreatedAt { get; set; }

        // ? THÊM properties b? thi?u:
        public string? Description { get; set; }
    }

    // Service Detail
    public class ServiceDetailViewModel
    {
        public Service Service { get; set; } = new Service();
        public string CategoryName { get; set; } = "";
        public IEnumerable<AppointmentHistoryItem> AppointmentHistory { get; set; } = new List<AppointmentHistoryItem>();
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public int CustomerCount { get; set; }

        // ? THÊM PROPERTY B? THI?U:
        public List<Service> RelatedServices { get; set; } = new List<Service>();
    }

    public class AppointmentHistoryItem
    {
        public int AppointmentId { get; set; }
        public string CustomerName { get; set; } = "";
        public DateTime AppointmentDate { get; set; }
        public string StatusName { get; set; } = "";
        public decimal Price { get; set; }
    }

    // Service Form (Add/Edit)
    public class ServiceFormViewModel
    {
        public ServiceInputViewModel Input { get; set; } = new ServiceInputViewModel();
        public IEnumerable<SerCate> Categories { get; set; } = new List<SerCate>();
        public string CategoryName { get; set; } = "Ch?n danh m?c";
        public IEnumerable<ServiceChangeHistoryItem> ChangeHistory { get; set; } = new List<ServiceChangeHistoryItem>();

        // Computed properties for view
        public string StatusText => Input.IsActive == true ? "?ang ho?t ??ng" : "?ã t?m ng?ng";
        public string StatusClass => Input.IsActive == true ? "badge-success" : "badge-danger";
        public string FormattedPrice => (Input.Price ?? 0).ToString("N0") + " VN?";
        public string DurationText => (Input.DurationMinutes ?? 0) + " phút";
    }

    public class ServiceChangeHistoryItem
    {
        public DateTime ChangeDate { get; set; }
        public string UserName { get; set; } = "";
        public string ChangeDescription { get; set; } = "";
        public string ChangeType { get; set; } = ""; // Create, Update, Delete, StatusChange
    }

    // Service Dashboard
    public class ServiceDashboardViewModel
    {
        // ===== BASIC STATS =====
        public int TotalServices { get; set; }
        public int ActiveServices { get; set; }
        public int InactiveServices { get; set; }
        public int TotalCategories { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }


        // ===== CHART DATA =====
        public ChartDataViewModel TopServiceChart { get; set; } = new ChartDataViewModel();
        public ChartDataViewModel CategoryChart { get; set; } = new ChartDataViewModel();

        // ===== COLLECTIONS =====
        public IEnumerable<TopServiceItem> TopServices { get; set; } = new List<TopServiceItem>();
        public IEnumerable<CategoryStatsItem> CategoryStats { get; set; } = new List<CategoryStatsItem>();
        public ServiceTrendData TrendData { get; set; } = new ServiceTrendData();

        // ? THÊM CÁC PROPERTIES B? THI?U:
        public IEnumerable<Service> RecentServices { get; set; } = new List<Service>();
        public Dictionary<string, int> CategoryDistribution { get; set; } = new Dictionary<string, int>();
    }


    public class TopServiceItem
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }

    public class CategoryStatsItem
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public int ServiceCount { get; set; }
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ChartDataViewModel
    {
        public string[] Labels { get; set; } = Array.Empty<string>();
        public int[] Data { get; set; } = Array.Empty<int>();
        public string ChartType { get; set; } = "";
        public string Title { get; set; } = "";
    }

    public class ServiceTrendData
    {
        public int NewServicesThisMonth { get; set; }
        public int NewServicesLastMonth { get; set; }
        public decimal RevenueGrowth { get; set; }
        public decimal BookingGrowth { get; set; }
        public string TrendDirection { get; set; } = "up";
    }

    // Service Category
    public class ServiceCategoryViewModel
    {
        public IEnumerable<SerCate> Categories { get; set; } = new List<SerCate>();
        public Dictionary<int, int> ServiceCountsByCategory { get; set; } = new Dictionary<int, int>();

        // Computed properties
        public int TotalCategories => Categories?.Count() ?? 0;
        public int ActiveCategories => Categories?.Count(c => c.IsActive == true) ?? 0;
        public int InactiveCategories => Categories?.Count(c => c.IsActive != true) ?? 0;
        public int TotalServices => ServiceCountsByCategory?.Values.Sum() ?? 0;
    }

    // Common models
    public class ServiceFilterModel
    {
        public int? CategoryId { get; set; }
        public string? Search { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public string? Sort { get; set; } = "name_asc";
        public string? Status { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }

    public class PaginationModel
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class ServiceSummaryStats
    {
        public int TotalServices { get; set; }
        public int ActiveServices { get; set; }
        public int InactiveServices { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
    }

    // Legacy support - ?? compatibility v?i code c?
    public class ServiceViewModel
    {
        // ? XÓA các properties c?:
        // (Legacy properties removed for maintainability)

        // ? THÊM properties gi?ng ServiceListViewModel:
        public IEnumerable<ServiceListItem> Services { get; set; } = new List<ServiceListItem>();
        public IEnumerable<SerCate> Categories { get; set; } = new List<SerCate>();
        public ServiceFilterModel Filter { get; set; } = new();
        public PaginationModel Pagination { get; set; } = new();
        public ServiceSummaryStats Summary { get; set; } = new();

        // Computed properties for backward compatibility
        public int TotalServices => Summary?.TotalServices ?? 0;
        public int ActiveServices => Summary?.ActiveServices ?? 0;
        public int InactiveServices => Summary?.InactiveServices ?? 0;
    }

    // Service Input ViewModel (for Add/Edit forms)
    public class ServiceInputViewModel
    {
        public int? ServiceId { get; set; }

        [Required(ErrorMessage = "Tên d?ch v? không ???c ?? tr?ng")]
        [StringLength(100, ErrorMessage = "Tên d?ch v? t?i ?a 100 ký t?")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Giá d?ch v? không ???c ?? tr?ng")]
        [Range(1000, 100000000, ErrorMessage = "Giá d?ch v? ph?i l?n h?n 0")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Vui lòng ch?n danh m?c")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng ch?n danh m?c h?p l?")]
        public int? CategoryId { get; set; }

        [StringLength(1000, ErrorMessage = "Mô t? t?i ?a 1000 ký t?")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Th?i gian th?c hi?n không ???c ?? tr?ng")]
        [Range(5, 480, ErrorMessage = "Th?i gian th?c hi?n ph?i t? 5 ??n 480 phút")]
        public int? DurationMinutes { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

