using System.Collections.Generic;
using System.Linq;

namespace pet_spa_system1.ViewModels
{
    public class ServiceListViewModel
    {
        public List<Models.Service> Services { get; set; } = new List<Models.Service>();
        public List<Models.SerCate> Categories { get; set; } = new List<Models.SerCate>();
        public int? SelectedCategoryId { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public string SortOrder { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = string.Empty;
        
        // Pagination properties
        public PageInfo PageInfo { get; set; } = new PageInfo();
        
        // Statistics
        public int TotalServices => Services?.Count ?? 0;
        public int ActiveServices => Services?.Count(s => s.IsActive == true) ?? 0;
        public int InactiveServices => Services?.Count(s => s.IsActive != true) ?? 0;
    }

    public class PageInfo
    {
        public int CurrentPage { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 10;
        public int TotalItems { get; set; } = 0;
        public int TotalPages => (TotalItems + ItemsPerPage - 1) / ItemsPerPage;
    }
}
