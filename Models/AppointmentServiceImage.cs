using System;

namespace pet_spa_system1.Models;

public partial class AppointmentServiceImage
{
    public int ImageId { get; set; }

    public int AppointmentServiceId { get; set; }

    public string ImgUrl { get; set; } = null!;
    public int? PetId { get; set; } // Thêm cột mới (nullable nếu DB cho phép)
    public DateTime CreatedAt { get; set; }
    public string? PhotoType { get; set; } = "Before"; // Optional: default value
    public virtual AppointmentService AppointmentService { get; set; } = null!;
    public virtual Pet? Pet { get; set; } // Navigation property nếu có bảng Pets
}
