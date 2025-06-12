using System;
using System.Collections.Generic;

namespace PetShopDemo_1.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? DurationMinutes { get; set; }

    public int CategoryId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    public virtual SerCate Category { get; set; } = null!;

    public virtual ICollection<PromotionService> PromotionServices { get; set; } = new List<PromotionService>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
