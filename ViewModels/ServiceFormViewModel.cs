using System.Collections.Generic;

namespace pet_spa_system1.ViewModels
{
    public class ServiceFormViewModel
    {
        public Models.Service Service { get; set; } = new Models.Service();
        public List<Models.SerCate> Categories { get; set; } = new List<Models.SerCate>();
        public string CategoryName { get; set; } = "Chọn danh mục";
        public List<ServiceChangeHistoryItem> ChangeHistory { get; set; } = new List<ServiceChangeHistoryItem>();
    }
}