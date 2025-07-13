using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pet_spa_system1.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    [Required(ErrorMessage = "Tên dịch vụ là bắt buộc")]
    [MaxLength(100, ErrorMessage = "Tên dịch vụ không được quá 100 ký tự")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Giá dịch vụ là bắt buộc")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá dịch vụ phải lớn hơn 0")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Thời gian thực hiện phải lớn hơn 0")]
    public int? DurationMinutes { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc")]
    public int CategoryId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

    public virtual SerCate Category { get; set; } = null!;

    public virtual ICollection<PromotionService> PromotionServices { get; set; } = new List<PromotionService>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
