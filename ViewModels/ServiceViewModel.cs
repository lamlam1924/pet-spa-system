using System.Collections.Generic;
using System.Linq;

namespace pet_spa_system1.ViewModels
{
    public class ServiceViewModel
    {
        public List<Models.Service> Services { get; set; } = new List<Models.Service>();
        public List<Models.SerCate> Categories { get; set; } = new List<Models.SerCate>();
        public int? SelectedCategoryId { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public string SortOrder { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = string.Empty;
        
        // Thống kê tổng quan tự động tính toán
        public int TotalServices => Services?.Count ?? 0;
        public int ActiveServices => Services?.Count(s => s.IsActive == true) ?? 0;
        public int InactiveServices => Services?.Count(s => s.IsActive != true) ?? 0;
    }
}