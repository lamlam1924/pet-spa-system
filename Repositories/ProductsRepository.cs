using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly PetDataShopContext _context;

        public ProductRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync(int page, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.Category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<List<ProductCategory>> GetAllProductCategoriesAsync()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        public async Task<List<Product>> GetActiveProductsAsync(int page, int pageSize)
        {
            return await _context.Products
                .Where(p => p.IsActive == true)
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountActiveProductsAsync()
        {
            return await _context.Products.CountAsync(p => p.IsActive == true);
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }


        public async Task DisableProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                product.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task EnsableProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                product.IsActive = true;
                await _context.SaveChangesAsync();
            }
        }

        public bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<List<Product>> GetSuggestedProductsAsync(int categoryId, int excludeProductId, int count)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.ProductId != excludeProductId && (p.IsActive ?? true))
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<ProductWithRatingViewModel>> GetActiveProductsWithRatingAsync(int page, int pageSize)
        {
            return await _context.Products
                .Where(p => p.IsActive == true)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductWithRatingViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    AverageRating = p.Reviews
                        .Where(r => r.Status == "Approved")
                        .Any() ? (int)Math.Round(p.Reviews
                        .Where(r => r.Status == "Approved")
                        .Average(r => r.Rating)) : 0,
                    ReviewCount = p.Reviews.Count(r => r.Status == "Approved")
                })
                .ToListAsync();
        }
        public async Task<Product?> GetProductWithReviewsByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Reviews.Where(r => r.Status == "Approved"))
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

        }

        public async Task<List<ProductWithRatingViewModel>> GetActiveProductsWithRatingAsync(int page, int pageSize, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, string sort = null)
{
    var query = _context.Products
        .Where(p => p.IsActive == true);

    if (categoryId.HasValue)
        query = query.Where(p => p.CategoryId == categoryId.Value);
    if (minPrice.HasValue)
        query = query.Where(p => p.Price >= minPrice.Value);
    if (maxPrice.HasValue)
        query = query.Where(p => p.Price <= maxPrice.Value);

    // Sắp xếp theo sort
    switch (sort)
    {
        case "price_asc":
            query = query.OrderBy(p => p.Price);
            break;
        case "price_desc":
            query = query.OrderByDescending(p => p.Price);
            break;
        case "best_rating":
            query = query.OrderByDescending(p => p.Reviews.Where(r => r.Status == "Approved").Any() ? p.Reviews.Where(r => r.Status == "Approved").Average(r => r.Rating) : 0);
            break;
        default:
            query = query.OrderByDescending(p => p.CreatedAt);
            break;
    }

    return await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new ProductWithRatingViewModel
        {
            ProductId = p.ProductId,
            Name = p.Name,
            ImageUrl = p.ImageUrl,
            Price = p.Price,
            CategoryName = p.Category.Name,
            AverageRating = p.Reviews
                .Where(r => r.Status == "Approved")
                .Any() ? (int)Math.Round(p.Reviews
                .Where(r => r.Status == "Approved")
                .Average(r => r.Rating)) : 0,
            ReviewCount = p.Reviews.Count(r => r.Status == "Approved")
        })
        .ToListAsync();
}

public async Task<int> CountActiveProductsAsync(int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null)
{
    var query = _context.Products.Where(p => p.IsActive == true);

    if (categoryId.HasValue)
        query = query.Where(p => p.CategoryId == categoryId.Value);
    if (minPrice.HasValue)
        query = query.Where(p => p.Price >= minPrice.Value);
    if (maxPrice.HasValue)
        query = query.Where(p => p.Price <= maxPrice.Value);

    return await query.CountAsync();
}

        public async Task AddProductReviewAsync(int userId, int productId, int rating, string comment, bool isAnonymous)
        {
            // Kiểm tra đã có review chưa
            var existingReview = await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
            if (existingReview != null)
            {
                // Update review cũ
                existingReview.Rating = rating;
                existingReview.Comment = comment;
                existingReview.Status = "Approved";
                existingReview.CreatedAt = DateTime.Now;
                // Nếu có trường IsAnonymous thì update luôn
                var prop = existingReview.GetType().GetProperty("IsAnonymous");
                if (prop != null)
                {
                    prop.SetValue(existingReview, isAnonymous);
                }
            }
            else
            {
                // Thêm review mới
                var review = new Review
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = rating,
                    Comment = comment,
                    Status = "Approved",
                    CreatedAt = DateTime.Now,
                };
                var prop = review.GetType().GetProperty("IsAnonymous");
                if (prop != null)
                {
                    prop.SetValue(review, isAnonymous);
                }
                _context.Reviews.Add(review);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Review>> GetRepliesForReviewAsync(int parentReviewId)
        {
            return await _context.Reviews
                .Where(r => r.ParentReviewId == parentReviewId)
                .Include(r => r.User)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task AddReplyToReviewAsync(int parentReviewId, int userId, string content)
        {
            // Truy ngược lên review cha gốc
            var parentReview = await _context.Reviews.FindAsync(parentReviewId);
            if (parentReview == null)
                throw new Exception("Parent review not found!");

            // Chặn reply vào bình luận của chính mình
            if (parentReview.UserId == userId)
                throw new Exception("Bạn không thể trả lời bình luận của chính mình.");

            var rootReview = parentReview;
            while (rootReview.ParentReviewId != null)
            {
                rootReview = await _context.Reviews.FindAsync(rootReview.ParentReviewId);
                if (rootReview == null)
                    throw new Exception("Root review not found!");
            }

            var now = DateTime.Now;
            // Không cần kiểm tra existed nữa, rely on unique index
            var reply = new Review
            {
                ParentReviewId = parentReviewId,
                UserId = userId,
                Comment = content,
                CreatedAt = now,
                Status = "Approved",
                Rating = 5 // Giá trị hợp lệ cho CHECK constraint
            };
            if (rootReview.ProductId != null)
            {
                reply.ProductId = rootReview.ProductId;
                reply.ServiceId = null;
            }
            else if (rootReview.ServiceId != null)
            {
                reply.ServiceId = rootReview.ServiceId;
                reply.ProductId = null;
            }
            else
            {
                throw new Exception("Không thể trả lời bình luận này vì không xác định được loại sản phẩm/dịch vụ.");
            }
            _context.Reviews.Add(reply);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Reviews_User_Parent_Comment"))
                    return;
                throw;
            }
        }

        public async Task<Review> GetLastReplyOfUserForParentAsync(int parentReviewId, int userId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ParentReviewId == parentReviewId && r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();
        }

    }
}
