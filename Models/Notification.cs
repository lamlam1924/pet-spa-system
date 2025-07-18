using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; } // liên kết với bảng Users
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        // Navigation property (nếu có)
        public User User { get; set; } // Nếu bạn có class User
    }
}
