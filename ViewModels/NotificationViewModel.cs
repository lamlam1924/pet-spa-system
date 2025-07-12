using System;

namespace pet_spa_system1.ViewModel
{
    public class NotificationViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
