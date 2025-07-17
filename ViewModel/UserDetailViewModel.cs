using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class UserDetailViewModel
    {
        public User User { get; set; }
        public List<Pet> Pets { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<Order> Orders { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Payment> Payments { get; set; }
        // Thêm các thống kê nếu cần
    }
} 