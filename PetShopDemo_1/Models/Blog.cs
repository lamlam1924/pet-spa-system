using System;
using System.Collections.Generic;

namespace PetShopDemo_1.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? ContentFormat { get; set; }

    public string? Category { get; set; }

    public string? Status { get; set; }

    public DateTime? PublishedAt { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual ICollection<BlogImage> BlogImages { get; set; } = new List<BlogImage>();

    public virtual User User { get; set; } = null!;
}
