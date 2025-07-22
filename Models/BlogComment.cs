using System;
using System.Collections.Generic;

namespace pet_spa_system1.Models;

public partial class BlogComment
{
    public int CommentId { get; set; }

    public int BlogId { get; set; }

    public int UserId { get; set; }

    public int? ParentCommentId { get; set; }

    public string Content { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual ICollection<BlogComment> Replies { get; set; } = new List<BlogComment>();

    public virtual BlogComment? ParentComment { get; set; }

    public virtual User User { get; set; } = null!;
}
