using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pet_spa_system1.Models;

public partial class Product
{
    public int ProductId { get; set; }
    [Required(ErrorMessage = "Phải chọn loại sản phẩm")]
    public int CategoryId { get; set; }
    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Giá tiền phải lớn hơn hoặc bằng 0")]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public int Stock { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    
    public virtual ProductCategory? Category { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
