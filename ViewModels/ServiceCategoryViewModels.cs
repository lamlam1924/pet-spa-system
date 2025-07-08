using pet_spa_system1.Models;
using System.Collections.Generic;
using System.Linq;

namespace pet_spa_system1.ViewModels
{
    public class ServiceCategoryViewModel
    {
        public List<SerCate> Categories { get; set; } = new List<SerCate>();
        public Dictionary<int, int> ServiceCountsByCategory { get; set; } = new Dictionary<int, int>();
        
        // Thống kê tổng quan
        public int TotalCategories => Categories?.Count ?? 0;
        public int ActiveCategories => Categories?.Count(c => c.IsActive == true) ?? 0;
        public int InactiveCategories => Categories?.Count(c => c.IsActive != true) ?? 0;
        public int TotalServices => ServiceCountsByCategory?.Values.Sum() ?? 0;
    }
}