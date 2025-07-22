using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Repositories;

public interface IBlogRepository
{
    Task<List<Blog>> GetAllBlogsAsync();
    Task<List<Blog>> GetPublishedBlogsAsync(int page, int pageSize, string? category = null, string? search = null, string sortBy = "newest");
    Task<List<Blog>> GetBlogsByUserAsync(int userId);
    Task<List<Blog>> GetPendingBlogsAsync();
    Task<Blog?> GetBlogByIdAsync(int blogId);
    Task<Blog?> GetBlogByIdWithDetailsAsync(int blogId);
    Task<int> AddBlogAsync(Blog blog);
    Task UpdateBlogAsync(Blog blog);
    Task DeleteBlogAsync(int blogId);
    Task<int> CountPublishedBlogsAsync(string? category = null, string? search = null);

    Task ApproveBlogAsync(int blogId, int approvedBy);
    Task RejectBlogAsync(int blogId, int approvedBy);
    Task PublishBlogAsync(int blogId);
    Task ArchiveBlogAsync(int blogId);

    Task AddBlogImageAsync(BlogImage blogImage);
    Task<List<BlogImage>> GetBlogImagesAsync(int blogId);
    Task DeleteBlogImageAsync(int imageId);

    Task<List<BlogComment>> GetBlogCommentsAsync(int blogId);
    Task<BlogComment?> GetCommentByIdAsync(int commentId);
    Task<int> AddCommentAsync(BlogComment comment);
    Task UpdateCommentAsync(BlogComment comment);
    Task DeleteCommentAsync(int commentId);
    Task ApproveCommentAsync(int commentId);
    Task RejectCommentAsync(int commentId);
    Task<int> CountBlogCommentsAsync(int blogId);

    Task<bool> IsLikedByUserAsync(int blogId, int userId);
    Task<int> CountBlogLikesAsync(int blogId);
    Task AddLikeAsync(int blogId, int userId);
    Task RemoveLikeAsync(int blogId, int userId);
    Task<List<BlogLike>> GetBlogLikesAsync(int blogId);

    Task<List<string>> GetBlogCategoriesAsync();

    Task<int> CountBlogsByStatusAsync(string status);
    Task<List<Blog>> GetRecentBlogsAsync(int count = 5);
    Task<List<Blog>> GetPopularBlogsAsync(int count = 5);
    Task<List<Blog>> GetRelatedBlogsAsync(int blogId, string? category = null, int count = 3);
    Task<string> GetUserRoleAsync(int userId);
}