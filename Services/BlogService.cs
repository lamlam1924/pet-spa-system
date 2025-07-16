using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModel;
using System.Text.RegularExpressions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace pet_spa_system1.Services;

public class BlogService : IBlogService
{
    private readonly IBlogRepository _blogRepository;

    public BlogService(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<BlogListViewModel> GetBlogListAsync(int page = 1, int pageSize = 10, string? category = null, string? search = null, string sortBy = "newest")
    {
        var blogs = await _blogRepository.GetPublishedBlogsAsync(page, pageSize, category, search, sortBy);
        var totalBlogs = await _blogRepository.CountPublishedBlogsAsync(category, search);
        var categories = await _blogRepository.GetBlogCategoriesAsync();

        var blogViewModels = blogs.Select(b => new BlogViewModel
        {
            BlogId = b.BlogId,
            Title = b.Title,
            Content = b.Content,
            ShortContent = GetShortContent(b.Content),
            Category = b.Category,
            Status = b.Status,
            PublishedAt = b.PublishedAt,
            CreatedAt = b.CreatedAt,
            AuthorName = b.User.FullName ?? b.User.Username,
            AuthorRole = GetUserRoleName(b.User.RoleId),
            FeaturedImageUrl = b.BlogImages.FirstOrDefault()?.ImageUrl,
            CommentCount = b.BlogComments.Count(c => c.Status == "Approved"),
            LikeCount = b.BlogLikes.Count
        }).ToList();

        return new BlogListViewModel
        {
            Blogs = blogViewModels,
            Categories = categories,
            CurrentPage = page,
            PageSize = pageSize,
            TotalBlogs = totalBlogs,
            TotalPages = (int)Math.Ceiling((double)totalBlogs / pageSize),
            SearchQuery = search,
            SelectedCategory = category,
            SortBy = sortBy
        };
    }

    public async Task<BlogDetailViewModel?> GetBlogDetailAsync(int blogId, int? currentUserId = null)
    {
        var blog = await _blogRepository.GetBlogByIdWithDetailsAsync(blogId);
        if (blog == null) return null;

        var blogViewModel = new BlogViewModel
        {
            BlogId = blog.BlogId,
            Title = blog.Title,
            Content = blog.Content,
            Category = blog.Category,
            Status = blog.Status,
            PublishedAt = blog.PublishedAt,
            CreatedAt = blog.CreatedAt,
            AuthorName = blog.User.FullName ?? blog.User.Username,
            AuthorRole = GetUserRoleName(blog.User.RoleId),
            FeaturedImageUrl = blog.BlogImages.FirstOrDefault()?.ImageUrl,
            CommentCount = blog.BlogComments.Count(c => c.Status == "Approved"),
            LikeCount = blog.BlogLikes.Count,
            IsLikedByCurrentUser = currentUserId.HasValue && blog.BlogLikes.Any(l => l.UserId == currentUserId.Value)
        };

        var comments = blog.BlogComments
            .Where(c => c.Status == "Approved" && c.ParentCommentId == null)
            .Select(c => MapToCommentViewModel(c))
            .ToList();

        var relatedBlogs = await GetRelatedBlogsAsync(blogId);

        return new BlogDetailViewModel
        {
            Blog = blogViewModel,
            Comments = comments,
            RelatedBlogs = relatedBlogs
        };
    }

    public async Task<BlogCreateViewModel> GetBlogCreateViewModelAsync()
    {
        var categories = await _blogRepository.GetBlogCategoriesAsync();

        return new BlogCreateViewModel
        {
            AvailableCategories = categories
        };
    }

    public async Task<int> CreateBlogAsync(BlogCreateViewModel model, int userId)
    {
        var blog = new Blog
        {
            UserId = userId,
            Title = model.Title,
            Content = model.Content,
            Category = model.Category,
            CreatedAt = DateTime.Now
        };

        var userRole = await GetUserRole(userId);
        blog.Status = userRole == "Customer" ? "PendingApproval" : "Published";
        if (blog.Status == "Published")
        {
            blog.PublishedAt = DateTime.Now;
        }

        var blogId = await _blogRepository.AddBlogAsync(blog);

        if (model.FeaturedImage != null)
        {
            var imageUrl = await UploadImageAsync(model.FeaturedImage);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                await _blogRepository.AddBlogImageAsync(new BlogImage
                {
                    BlogId = blogId,
                    ImageUrl = imageUrl,
                    DisplayOrder = 0,
                    CreatedAt = DateTime.Now
                });
            }
        }

        if (model.AdditionalImages != null && model.AdditionalImages.Any())
        {
            int displayOrder = 1;
            foreach (var image in model.AdditionalImages)
            {
                var imageUrl = await UploadImageAsync(image);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    await _blogRepository.AddBlogImageAsync(new BlogImage
                    {
                        BlogId = blogId,
                        ImageUrl = imageUrl,
                        DisplayOrder = displayOrder++,
                        CreatedAt = DateTime.Now
                    });
                }
            }
        }

        return blogId;
    }

    public async Task<bool> UpdateBlogAsync(int blogId, BlogCreateViewModel model, int userId)
    {
        var blog = await _blogRepository.GetBlogByIdAsync(blogId);
        if (blog == null || !CanUserEditBlog(blogId, userId, await GetUserRole(userId)))
            return false;

        blog.Title = model.Title;
        blog.Content = model.Content;
        blog.Category = model.Category;

        if (model.Status == "Published" && blog.Status != "Published")
        {
            blog.Status = "Published";
            blog.PublishedAt = DateTime.Now;
        }
        else if (model.Status != "Published")
        {
            blog.Status = model.Status;
        }

        await _blogRepository.UpdateBlogAsync(blog);

        if (model.FeaturedImage != null)
        {
            var imageUrl = await UploadImageAsync(model.FeaturedImage);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                await _blogRepository.AddBlogImageAsync(new BlogImage
                {
                    BlogId = blogId,
                    ImageUrl = imageUrl,
                    DisplayOrder = 0,
                    CreatedAt = DateTime.Now
                });
            }
        }

        return true;
    }

    public async Task<bool> DeleteBlogAsync(int blogId, int userId)
    {
        if (!CanUserDeleteBlog(blogId, userId, await GetUserRole(userId)))
            return false;

        await _blogRepository.DeleteBlogAsync(blogId);
        return true;
    }


    public async Task<bool> PublishBlogAsync(int blogId, int userId)
    {
        await _blogRepository.PublishBlogAsync(blogId);
        return true;
    }

    public async Task<bool> ArchiveBlogAsync(int blogId, int userId)
    {
        await _blogRepository.ArchiveBlogAsync(blogId);
        return true;
    }

    public async Task<bool> AddCommentAsync(int blogId, string content, int userId, int? parentCommentId = null)
    {
        var comment = new BlogComment
        {
            BlogId = blogId,
            UserId = userId,
            ParentCommentId = parentCommentId,
            Content = content,
            Status = "Approved",
            CreatedAt = DateTime.Now
        };

        await _blogRepository.AddCommentAsync(comment);
        return true;
    }

    public async Task<bool> UpdateCommentAsync(int commentId, string content, int userId)
    {
        var comment = await _blogRepository.GetCommentByIdAsync(commentId);
        if (comment == null || comment.UserId != userId)
            return false;

        comment.Content = content;
        await _blogRepository.UpdateCommentAsync(comment);
        return true;
    }

    public async Task<bool> DeleteCommentAsync(int commentId, int userId)
    {
        var comment = await _blogRepository.GetCommentByIdAsync(commentId);
        if (comment == null || (comment.UserId != userId && (await GetUserRole(userId)) != "Admin"))
            return false;

        await _blogRepository.DeleteCommentAsync(commentId);
        return true;
    }

    public async Task<bool> ApproveCommentAsync(int commentId, int adminId)
    {
        await _blogRepository.ApproveCommentAsync(commentId);
        return true;
    }

    public async Task<bool> RejectCommentAsync(int commentId, int adminId)
    {
        await _blogRepository.RejectCommentAsync(commentId);
        return true;
    }

    public async Task<bool> ToggleLikeAsync(int blogId, int userId)
    {
        var isLiked = await _blogRepository.IsLikedByUserAsync(blogId, userId);

        if (isLiked)
        {
            await _blogRepository.RemoveLikeAsync(blogId, userId);
        }
        else
        {
            await _blogRepository.AddLikeAsync(blogId, userId);
        }

        return !isLiked;
    }

    public async Task<bool> IsLikedByUserAsync(int blogId, int userId)
    {
        return await _blogRepository.IsLikedByUserAsync(blogId, userId);
    }

    public async Task<int> GetLikeCountAsync(int blogId)
    {
        return await _blogRepository.CountBlogLikesAsync(blogId);
    }

    public async Task<BlogAdminViewModel> GetAdminDashboardAsync()
    {
        var pendingBlogs = await GetPendingBlogsAsync();
        var totalBlogs = await _blogRepository.CountBlogsByStatusAsync("Published");
        var pendingCount = await _blogRepository.CountBlogsByStatusAsync("PendingApproval");
        var rejectedCount = await _blogRepository.CountBlogsByStatusAsync("Rejected");

//        return new BlogAdminViewModel
//        {
//            PendingBlogs = pendingBlogs,
//            TotalBlogs = totalBlogs,
//            PendingApproval = pendingCount,
//            PublishedBlogs = totalBlogs,
//            RejectedBlogs = rejectedCount
//        };
//    }

//    public async Task<List<BlogViewModel>> GetPendingBlogsAsync()
//    {
//        var blogs = await _blogRepository.GetPendingBlogsAsync();
//        return blogs.Select(b => new BlogViewModel
//        {
//            BlogId = b.BlogId,
//            Title = b.Title,
//            Content = b.Content,
//            ShortContent = GetShortContent(b.Content),
//            Category = b.Category,
//            Status = b.Status,
//            CreatedAt = b.CreatedAt,
//            AuthorName = b.User.FullName ?? b.User.Username,
//            AuthorRole = GetUserRoleName(b.User.RoleId),
//            FeaturedImageUrl = b.BlogImages.FirstOrDefault()?.ImageUrl
//        }).ToList();
//    }

//    public async Task<List<BlogViewModel>> GetAllBlogsForAdminAsync()
//    {
//        var blogs = await _blogRepository.GetAllBlogsAsync();
//        return blogs.Select(b => new BlogViewModel
//        {
//            BlogId = b.BlogId,
//            Title = b.Title,
//            Content = b.Content,
//            ShortContent = GetShortContent(b.Content),
//            Category = b.Category,
//            Status = b.Status,
//            CreatedAt = b.CreatedAt,
//            PublishedAt = b.PublishedAt,
//            AuthorName = b.User.FullName ?? b.User.Username,
//            AuthorRole = GetUserRoleName(b.User.RoleId),
//            FeaturedImageUrl = b.BlogImages.FirstOrDefault()?.ImageUrl,
//            CommentCount = b.BlogComments.Count(c => c.Status == "Approved"),
//            LikeCount = b.BlogLikes.Count
//        }).ToList();
//    }

//    public async Task<List<BlogViewModel>> GetBlogsByUserAsync(int userId)
//    {
//        var blogs = await _blogRepository.GetBlogsByUserAsync(userId);
//        return blogs.Select(b => new BlogViewModel
//        {
//            BlogId = b.BlogId,
//            Title = b.Title,
//            Content = b.Content,
//            ShortContent = GetShortContent(b.Content),
//            Category = b.Category,
//            Status = b.Status,
//            PublishedAt = b.PublishedAt,
//            CreatedAt = b.CreatedAt,
//            FeaturedImageUrl = b.BlogImages.FirstOrDefault()?.ImageUrl,
//            CommentCount = b.BlogComments.Count(c => c.Status == "Approved"),
//            LikeCount = b.BlogLikes.Count
//        }).ToList();
//    }

//    public async Task<bool> ApproveBlogAsync(int blogId, int adminUserId)
//    {
//        var blog = await _blogRepository.GetBlogByIdAsync(blogId);
//        if (blog == null || blog.Status != "PendingApproval")
//            return false;

//        blog.Status = "Published";
//        blog.PublishedAt = DateTime.Now;
//        blog.ApprovedBy = adminUserId;

//        await _blogRepository.UpdateBlogAsync(blog);
//        return true;
//    }

//    public async Task<bool> RejectBlogAsync(int blogId, int adminUserId, string? reason = null)
//    {
//        var blog = await _blogRepository.GetBlogByIdAsync(blogId);
//        if (blog == null || blog.Status != "PendingApproval")
//            return false;

//        blog.Status = "Rejected";
//        blog.ApprovedBy = adminUserId;

//        await _blogRepository.UpdateBlogAsync(blog);
//        return true;
//    }

//    public async Task<List<string>> GetCategoriesAsync()
//    {
//        return await _blogRepository.GetBlogCategoriesAsync();
//    }

//    public async Task<List<BlogViewModel>> GetRecentBlogsAsync(int count = 5)
//    {
//        var blogs = await _blogRepository.GetRecentBlogsAsync(count);
//        return blogs.Select(b => new BlogViewModel
//        {
//            BlogId = b.BlogId,
//            Title = b.Title,
//            ShortContent = GetShortContent(b.Content),
//            Category = b.Category,
//            PublishedAt = b.PublishedAt,
//            AuthorName = b.User.FullName ?? b.User.Username,
//            FeaturedImageUrl = b.BlogImages.FirstOrDefault()?.ImageUrl,
//            CommentCount = b.BlogComments.Count(c => c.Status == "Approved"),
//            LikeCount = b.BlogLikes.Count
//        }).ToList();
//    }

//    public async Task<List<BlogViewModel>> GetPopularBlogsAsync(int count = 5)
//    {
//        var blogs = await _blogRepository.GetPopularBlogsAsync(count);
//        return blogs.Select(b => new BlogViewModel
//        {
//            BlogId = b.BlogId,
//            Title = b.Title,
//            ShortContent = GetShortContent(b.Content),
//            Category = b.Category,
//            PublishedAt = b.PublishedAt,
//            AuthorName = b.User.FullName ?? b.User.Username,
//            FeaturedImageUrl = b.BlogImages.FirstOrDefault()?.ImageUrl,
//            CommentCount = b.BlogComments.Count(c => c.Status == "Approved"),
//            LikeCount = b.BlogLikes.Count
//        }).ToList();
//    }

//    public async Task<List<BlogViewModel>> GetRelatedBlogsAsync(int blogId, int count = 3)
//    {
//        var blog = await _blogRepository.GetBlogByIdAsync(blogId);
//        var relatedBlogs = await _blogRepository.GetRelatedBlogsAsync(blogId, blog?.Category, count);

//        return relatedBlogs.Select(b => new BlogViewModel
//        {
//            BlogId = b.BlogId,
//            Title = b.Title,
//            ShortContent = GetShortContent(b.Content),
//            Category = b.Category,
//            PublishedAt = b.PublishedAt,
//            AuthorName = b.User.FullName ?? b.User.Username,
//            FeaturedImageUrl = b.BlogImages.FirstOrDefault()?.ImageUrl
//        }).ToList();
//    }

//    public async Task<string> UploadImageAsync(IFormFile file)
//    {
//        if (file == null || file.Length == 0)
//            return string.Empty;

//        try
//        {
//            var account = new Account(
//                "dprp1jbd9", 
//                "584135338254938", 
//                "QbUYngPIdZcXEn_mipYn8RE5dlo" 
//            );
//            var cloudinary = new Cloudinary(account);

//            var uploadParams = new ImageUploadParams()
//            {
//                File = new FileDescription(file.FileName, file.OpenReadStream()),
//                Folder = "blog_images", 
//                PublicId = $"blog_{Guid.NewGuid()}", 
//                Transformation = new Transformation()
//                    .Width(1200).Height(800).Crop("limit") 
//                    .Quality("auto") 
//                    .FetchFormat("auto") 
//            };

//            var uploadResult = await cloudinary.UploadAsync(uploadParams);

//            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
//            {
//                return uploadResult.SecureUrl.ToString();
//            }
//            else
//            {
//                throw new Exception($"Cloudinary upload failed: {uploadResult.Error?.Message}");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error uploading image to Cloudinary: {ex.Message}");
//            return string.Empty;
//        }
//    }

//    public async Task<bool> DeleteImageAsync(string imageUrl)
//    {
//        if (string.IsNullOrEmpty(imageUrl))
//            return false;

//        try
//        {
//            if (!imageUrl.Contains("cloudinary.com"))
//                return false;

//            var account = new Account(
//                "dprp1jbd9", 
//                "584135338254938", 
//                "QbUYngPIdZcXEn_mipYn8RE5dlo" 
//            );
//            var cloudinary = new Cloudinary(account);

//            var publicId = ExtractPublicIdFromUrl(imageUrl);
//            if (string.IsNullOrEmpty(publicId))
//                return false;

//            var deleteParams = new DeletionParams(publicId);
//            var result = await cloudinary.DestroyAsync(deleteParams);

//            return result.StatusCode == System.Net.HttpStatusCode.OK;
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error deleting image from Cloudinary: {ex.Message}");
//            return false;
//        }
//    }
//    private string ExtractPublicIdFromUrl(string imageUrl)
//    {
//        try
//        {
            
//            var uri = new Uri(imageUrl);
//            var segments = uri.AbsolutePath.Split('/');

//            var uploadIndex = Array.IndexOf(segments, "upload");
//            if (uploadIndex >= 0 && uploadIndex + 2 < segments.Length)
//            {
//                var pathParts = segments.Skip(uploadIndex + 2).ToArray();
//                var fullPath = string.Join("/", pathParts);

//                var lastDotIndex = fullPath.LastIndexOf('.');
//                if (lastDotIndex > 0)
//                {
//                    return fullPath.Substring(0, lastDotIndex);
//                }
//                return fullPath;
//            }

//            return string.Empty;
//        }
//        catch
//        {
//            return string.Empty;
//        }
//    }

//    public string GetShortContent(string content, int maxLength = 200)
//    {
//        if (string.IsNullOrEmpty(content))
//            return string.Empty;

//        var plainText = Regex.Replace(content, "<.*?>", string.Empty);

//        if (plainText.Length <= maxLength)
//            return plainText;

//        return plainText.Substring(0, maxLength) + "...";
//    }

//    public bool CanUserEditBlog(int blogId, int userId, string userRole)
//    {
//        if (userRole == "Admin")
//            return true;

    
//        return true; 
//    }

//    public bool CanUserDeleteBlog(int blogId, int userId, string userRole)
//    {
//        if (userRole == "Admin")
//            return true;

//        return true; 
//    }

//    public string GetBlogStatusForUser(string userRole)
//    {
//        return userRole switch
//        {
//            "Admin" => "Draft",
//            "Staff" => "Draft",
//            "Customer" => "PendingApproval",
//            _ => "PendingApproval"
//        };
//    }

//    private BlogCommentViewModel MapToCommentViewModel(BlogComment comment)
//    {
//        return new BlogCommentViewModel
//        {
//            CommentId = comment.CommentId,
//            BlogId = comment.BlogId,
//            Content = comment.Content,
//            Status = comment.Status ?? "Pending",
//            CreatedAt = comment.CreatedAt,
//            UserName = comment.User.FullName ?? comment.User.Username,
//            UserRole = GetUserRoleName(comment.User.RoleId),
//            UserAvatar = comment.User.ProfilePictureUrl,
//            ParentCommentId = comment.ParentCommentId,
//            Replies = comment.Replies.Where(r => r.Status == "Approved").Select(MapToCommentViewModel).ToList(),
//            CanReply = true
//        };
//    }

//    private async Task<string> GetUserRole(int userId)
//    {
//        return await _blogRepository.GetUserRoleAsync(userId);
//    }

//    private string GetUserRoleName(int roleId)
//    {
//        return roleId switch
//        {
//            1 => "Admin",
//            2 => "Customer",
//            3 => "Staff",
//            _ => "User"
//        };
//    }
//}
