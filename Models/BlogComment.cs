using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pet_spa_system1.Models;

public partial class BlogComment
{
    public int CommentId { get; set; }

    public int BlogId { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }

    [ForeignKey("ParentComment")]
    public int? ParentCommentId { get; set; }

    [Required]
    public string Content { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Blog Blog { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual BlogComment? ParentComment { get; set; }

    public virtual ICollection<BlogComment> Replies { get; set; } = new List<BlogComment>();
}