using System;

namespace pet_spa_system1.Models;

public partial class AppointmentServiceImage
{
    public int ImageId { get; set; }

    public int AppointmentServiceId { get; set; }

    public string ImgUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public string? PhotoType { get; set; } = "Before"; // Optional: default value
    public virtual AppointmentService AppointmentService { get; set; } = null!;
}
