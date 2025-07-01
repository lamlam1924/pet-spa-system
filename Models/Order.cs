using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public int StatusId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual StatusOrder Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
