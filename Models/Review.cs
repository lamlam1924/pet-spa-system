using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int? ProductId { get; set; }

    public int? ServiceId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ParentReviewId { get; set; }
    public Review? ParentReview { get; set; }
    public ICollection<Review>? Replies { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Service? Service { get; set; }

    public virtual User User { get; set; } = null!;
}
