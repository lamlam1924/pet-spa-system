using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Services;

public interface IBlogService
{
    Task<BlogListViewModel> GetBlogListAsync(int page = 1, int pageSize = 10, string? category = null, string? search = null, string sortBy = "newest");
    Task<BlogDetailViewModel?> GetBlogDetailAsync(int blogId, int? currentUserId = null);
    Task<BlogCreateViewModel> GetBlogCreateViewModelAsync();
    Task<int> CreateBlogAsync(BlogCreateViewModel model, int userId);
    Task<bool> UpdateBlogAsync(int blogId, BlogCreateViewModel model, int userId);
    Task<bool> DeleteBlogAsync(int blogId, int userId);

    Task<bool> PublishBlogAsync(int blogId, int userId);
    Task<bool> ArchiveBlogAsync(int blogId, int userId);

    Task<bool> AddCommentAsync(int blogId, string content, int userId, int? parentCommentId = null);
    Task<bool> UpdateCommentAsync(int commentId, string content, int userId);
    Task<bool> DeleteCommentAsync(int commentId, int userId);
    Task<bool> ApproveCommentAsync(int commentId, int adminId);
    Task<bool> RejectCommentAsync(int commentId, int adminId);

    Task<bool> ToggleLikeAsync(int blogId, int userId);
    Task<bool> IsLikedByUserAsync(int blogId, int userId);
    Task<int> GetLikeCountAsync(int blogId);

    Task<BlogAdminViewModel> GetAdminDashboardAsync();
    Task<List<BlogViewModel>> GetPendingBlogsAsync();
    Task<List<BlogViewModel>> GetAllBlogsForAdminAsync();
    Task<List<BlogViewModel>> GetBlogsByUserAsync(int userId);
    Task<bool> ApproveBlogAsync(int blogId, int adminUserId);
    Task<bool> RejectBlogAsync(int blogId, int adminUserId, string? reason = null);

    Task<List<string>> GetCategoriesAsync();
    Task<List<BlogViewModel>> GetRecentBlogsAsync(int count = 5);
    Task<List<BlogViewModel>> GetPopularBlogsAsync(int count = 5);
    Task<List<BlogViewModel>> GetRelatedBlogsAsync(int blogId, int count = 3);

    Task<string> UploadImageAsync(IFormFile file);
    Task<bool> DeleteImageAsync(string imageUrl);

    Task<List<BlogViewModel>> GetRejectedBlogsByUserAsync(int userId);

    string GetShortContent(string content, int maxLength = 200);
    bool CanUserEditBlog(int blogId, int userId, string userRole);
    bool CanUserDeleteBlog(int blogId, int userId, string userRole);
    string GetBlogStatusForUser(string userRole);
}

