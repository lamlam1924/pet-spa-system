using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModels
{
    public class ServiceViewModel
    {
        public List<Service> Services { get; set; } = new List<Service>();
        public List<SerCate> Categories { get; set; } = new List<SerCate>();
        public int? SelectedCategoryId { get; set; }
        public List<Promotion> Promotions { get; set; } = new List<Promotion>();
        public List<PromotionService> PromotionServices { get; set; } = new List<PromotionService>();
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}