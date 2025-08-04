using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel
{
    public class PetViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public string Gender { get; set; }
        public string HealthCondition { get; set; }
        public string Note { get; set; }
        public string ImageUrl { get; set; } // Thêm thuộc tính này
        public string Breed { get; set; }    // Thêm thuộc tính mới
        public int? Age { get; set; }        // Thêm thuộc tính mới, nullable để cho phép null
        public List<PetImage> PetImages { get; set; }
    }

    /// <summary>
    /// Thông tin về pet có lịch hẹn trùng thời gian
    /// </summary>
    public class PetConflictInfo
    {
        public int PetId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int ConflictingAppointmentId { get; set; }
        public DateTime ConflictingStartTime { get; set; }
        public DateTime ConflictingEndTime { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
    }
}