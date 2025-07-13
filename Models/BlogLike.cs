using System;
using System.ComponentModel.DataAnnotations;

namespace pet_spa_system1.Models;

public partial class BlogLike
{
    public int LikeId { get; set; }

    public int BlogId { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    // Navigation properties
    public virtual Blog Blog { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
