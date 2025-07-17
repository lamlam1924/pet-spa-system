using System.ComponentModel.DataAnnotations;
using pet_spa_system1.Models;

namespace pet_spa_system1.ViewModel;

public class BlogViewModel
{
    public int BlogId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorRole { get; set; } = string.Empty;
    
    public string? FeaturedImageUrl { get; set; }
    
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    
    public string ShortContent { get; set; } = string.Empty;
}

public class BlogListViewModel
{
    public List<BlogViewModel> Blogs { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    public int TotalBlogs { get; set; }
    
    public string? SearchQuery { get; set; }
    public string? SelectedCategory { get; set; }
    public string SortBy { get; set; } = "newest"; // newest, oldest, popular
}


public class BlogDetailViewModel
{
    public BlogViewModel Blog { get; set; } = new();
    public List<BlogCommentViewModel> Comments { get; set; } = new();
    public List<BlogViewModel> RelatedBlogs { get; set; } = new();
    
    public string NewCommentContent { get; set; } = string.Empty;
    public int? ReplyToCommentId { get; set; }
}

public class BlogCommentViewModel
{
    public int CommentId { get; set; }
    public int BlogId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    
    public string UserName { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    
    
    public int? ParentCommentId { get; set; }
    public List<BlogCommentViewModel> Replies { get; set; } = new();
    public bool CanReply { get; set; } = true;
}

public class BlogAdminViewModel
{
    public List<BlogViewModel> PendingBlogs { get; set; } = new();
    public List<BlogViewModel> AllBlogs { get; set; } = new();
    
    public int TotalBlogs { get; set; }
    public int PendingApproval { get; set; }
    public int PublishedBlogs { get; set; }
    public int RejectedBlogs { get; set; }
    
    public string StatusFilter { get; set; } = "All"; 
    public string? SearchQuery { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
