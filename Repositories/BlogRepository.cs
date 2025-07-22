using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Repositories;

public class BlogRepository : IBlogRepository
{
    private readonly PetDataShopContext _context;

    public BlogRepository(PetDataShopContext context)
    {
        _context = context;
    }

    public async Task<List<Blog>> GetAllBlogsAsync()
    {
        return await _context.Blogs
            .Include(b => b.User)
            .Include(b => b.ApprovedByNavigation)
            .Include(b => b.BlogImages)
            .Include(b => b.BlogComments)
            .Include(b => b.BlogLikes)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Blog>> GetPublishedBlogsAsync(int page, int pageSize, string? category = null, string? search = null, string sortBy = "newest")
    {
        var query = _context.Blogs
            .Include(b => b.User)
            .Include(b => b.BlogImages)
            .Include(b => b.BlogComments.Where(c => c.Status == "Approved"))
            .Include(b => b.BlogLikes)
            .Where(b => b.Status == "Published");

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(b => b.Category == category);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => b.Title.Contains(search) || b.Content.Contains(search));
        }
        query = sortBy.ToLower() switch
        {
            "oldest" => query.OrderBy(b => b.PublishedAt),
            "popular" => query.OrderByDescending(b => b.BlogLikes.Count).ThenByDescending(b => b.BlogComments.Count),
            _ => query.OrderByDescending(b => b.PublishedAt ?? b.CreatedAt)
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Blog>> GetBlogsByUserAsync(int userId)
    {
        return await _context.Blogs
            .Include(b => b.BlogImages)
            .Include(b => b.BlogComments)
            .Include(b => b.BlogLikes)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Blog>> GetPendingBlogsAsync()
    {
        return await _context.Blogs
            .Include(b => b.User)
            .Include(b => b.BlogImages)
            .Where(b => b.Status == "PendingApproval")
            .OrderBy(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Blog?> GetBlogByIdAsync(int blogId)
    {
        return await _context.Blogs
            .Include(b => b.User)
            .Include(b => b.ApprovedByNavigation)
            .Include(b => b.BlogImages)
            .FirstOrDefaultAsync(b => b.BlogId == blogId);
    }

    public async Task<Blog?> GetBlogByIdWithDetailsAsync(int blogId)
    {
        return await _context.Blogs
            .Include(b => b.User)
            .Include(b => b.ApprovedByNavigation)
            .Include(b => b.BlogImages)
            .Include(b => b.BlogComments.Where(c => c.Status == "Approved"))
                .ThenInclude(c => c.User)
            .Include(b => b.BlogComments.Where(c => c.Status == "Approved"))
                .ThenInclude(c => c.Replies.Where(r => r.Status == "Approved"))
                .ThenInclude(r => r.User)
            .Include(b => b.BlogLikes)
                .ThenInclude(l => l.User)
            .FirstOrDefaultAsync(b => b.BlogId == blogId);
    }

    public async Task<int> AddBlogAsync(Blog blog)
    {
        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync();
        return blog.BlogId;
    }

    public async Task UpdateBlogAsync(Blog blog)
    {
        blog.UpdatedAt = DateTime.Now;
        _context.Blogs.Update(blog);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBlogAsync(int blogId)
    {
        var blog = await _context.Blogs.FindAsync(blogId);
        if (blog != null)
        {
            // Xóa tất cả các lượt thích liên quan
            var likes = await _context.BlogLikes.Where(l => l.BlogId == blogId).ToListAsync();
            if (likes.Any())
            {
                _context.BlogLikes.RemoveRange(likes);
                await _context.SaveChangesAsync();
            }

            // Xóa tất cả các bình luận liên quan (bao gồm cả replies)
            var comments = await _context.BlogComments.Where(c => c.BlogId == blogId).ToListAsync();
            if (comments.Any())
            {
                _context.BlogComments.RemoveRange(comments);
                await _context.SaveChangesAsync();
            }

            // Xóa blog
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CountPublishedBlogsAsync(string? category = null, string? search = null)
    {
        var query = _context.Blogs.Where(b => b.Status == "Published");

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(b => b.Category == category);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => b.Title.Contains(search) || b.Content.Contains(search));
        }

        return await query.CountAsync();
    }

    public async Task ApproveBlogAsync(int blogId, int approvedBy)
    {
        var blog = await _context.Blogs.FindAsync(blogId);
        if (blog != null)
        {
            blog.Status = "Approved";
            blog.ApprovedBy = approvedBy;
            blog.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RejectBlogAsync(int blogId, int approvedBy)
    {
        var blog = await _context.Blogs.FindAsync(blogId);
        if (blog != null)
        {
            blog.Status = "Rejected";
            blog.ApprovedBy = approvedBy;
            blog.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task PublishBlogAsync(int blogId)
    {
        var blog = await _context.Blogs.FindAsync(blogId);
        if (blog != null)
        {
            blog.Status = "Published";
            blog.PublishedAt = DateTime.Now;
            blog.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ArchiveBlogAsync(int blogId)
    {
        var blog = await _context.Blogs.FindAsync(blogId);
        if (blog != null)
        {
            blog.Status = "Archived";
            blog.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddBlogImageAsync(BlogImage blogImage)
    {
        _context.BlogImages.Add(blogImage);
        await _context.SaveChangesAsync();
    }

    public async Task<List<BlogImage>> GetBlogImagesAsync(int blogId)
    {
        return await _context.BlogImages
            .Where(bi => bi.BlogId == blogId)
            .OrderBy(bi => bi.DisplayOrder)
            .ToListAsync();
    }

    public async Task DeleteBlogImageAsync(int imageId)
    {
        var image = await _context.BlogImages.FindAsync(imageId);
        if (image != null)
        {
            _context.BlogImages.Remove(image);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<BlogComment>> GetBlogCommentsAsync(int blogId)
    {
        return await _context.BlogComments
            .Include(c => c.User)
            .Include(c => c.Replies.Where(r => r.Status == "Approved"))
                .ThenInclude(r => r.User)
            .Where(c => c.BlogId == blogId && c.Status == "Approved" && c.ParentCommentId == null)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<BlogComment?> GetCommentByIdAsync(int commentId)
    {
        return await _context.BlogComments
            .Include(c => c.User)
            .Include(c => c.Blog)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);
    }

    public async Task<int> AddCommentAsync(BlogComment comment)
    {
        _context.BlogComments.Add(comment);
        await _context.SaveChangesAsync();
        return comment.CommentId;
    }

    public async Task UpdateCommentAsync(BlogComment comment)
    {
        comment.UpdatedAt = DateTime.Now;
        _context.BlogComments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(int commentId)
    {
        var comment = await _context.BlogComments.FindAsync(commentId);
        if (comment != null)
        {
            _context.BlogComments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ApproveCommentAsync(int commentId)
    {
        var comment = await _context.BlogComments.FindAsync(commentId);
        if (comment != null)
        {
            comment.Status = "Approved";
            comment.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RejectCommentAsync(int commentId)
    {
        var comment = await _context.BlogComments.FindAsync(commentId);
        if (comment != null)
        {
            comment.Status = "Rejected";
            comment.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CountBlogCommentsAsync(int blogId)
    {
        return await _context.BlogComments
            .Where(c => c.BlogId == blogId && c.Status == "Approved")
            .CountAsync();
    }

    public async Task<bool> IsLikedByUserAsync(int blogId, int userId)
    {
        return await _context.BlogLikes
            .AnyAsync(l => l.BlogId == blogId && l.UserId == userId);
    }

    public async Task<int> CountBlogLikesAsync(int blogId)
    {
        return await _context.BlogLikes
            .Where(l => l.BlogId == blogId)
            .CountAsync();
    }

    public async Task AddLikeAsync(int blogId, int userId)
    {
        var existingLike = await _context.BlogLikes
            .FirstOrDefaultAsync(l => l.BlogId == blogId && l.UserId == userId);

        if (existingLike == null)
        {
            _context.BlogLikes.Add(new BlogLike
            {
                BlogId = blogId,
                UserId = userId,
                CreatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveLikeAsync(int blogId, int userId)
    {
        var like = await _context.BlogLikes
            .FirstOrDefaultAsync(l => l.BlogId == blogId && l.UserId == userId);

        if (like != null)
        {
            _context.BlogLikes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<BlogLike>> GetBlogLikesAsync(int blogId)
    {
        return await _context.BlogLikes
            .Include(l => l.User)
            .Where(l => l.BlogId == blogId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<string>> GetBlogCategoriesAsync()
    {
        return await _context.Blogs
            .Where(b => !string.IsNullOrEmpty(b.Category) && b.Status == "Published")
            .Select(b => b.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
    public async Task<int> CountBlogsByStatusAsync(string status)
    {
        return await _context.Blogs
            .Where(b => b.Status == status)
            .CountAsync();
    }

    public async Task<List<Blog>> GetRecentBlogsAsync(int count = 5)
    {
        return await _context.Blogs
            .Include(b => b.User)
            .Include(b => b.BlogImages)
            .Where(b => b.Status == "Published")
            .OrderByDescending(b => b.PublishedAt ?? b.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Blog>> GetPopularBlogsAsync(int count = 5)
    {
        return await _context.Blogs
            .Include(b => b.User)
            .Include(b => b.BlogImages)
            .Include(b => b.BlogLikes)
            .Include(b => b.BlogComments)
            .Where(b => b.Status == "Published")
            .OrderByDescending(b => b.BlogLikes.Count + b.BlogComments.Count)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Blog>> GetRelatedBlogsAsync(int blogId, string? category = null, int count = 3)
    {
        var query = _context.Blogs
            .Include(b => b.User)
            .Include(b => b.BlogImages)
            .Where(b => b.Status == "Published" && b.BlogId != blogId);

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(b => b.Category == category);
        }

        return await query
            .OrderByDescending(b => b.PublishedAt ?? b.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<string> GetUserRoleAsync(int userId)
    {
        var user = await _context.Users
            .Where(u => u.UserId == userId)
            .Select(u => new { u.RoleId })
            .FirstOrDefaultAsync();

        return user != null ? GetUserRoleName(user.RoleId) : "Customer";
    }

    private string GetUserRoleName(int roleId)
    {
        return roleId switch
        {
            1 => "Admin",
            2 => "Customer",
            3 => "Staff",
            _ => "User"
        };
    }
}