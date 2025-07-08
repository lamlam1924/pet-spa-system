using System;

namespace pet_spa_system1.ViewModels
{
    public class ServiceChangeHistoryItem
    {
        public DateTime ChangeDate { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string ChangeDescription { get; set; } = string.Empty;
        
        // Factory method để tạo item mới
        public static ServiceChangeHistoryItem Create(DateTime date, string userName, string description)
        {
            return new ServiceChangeHistoryItem
            {
                ChangeDate = date,
                UserName = userName,
                ChangeDescription = description
            };
        }
    }
}