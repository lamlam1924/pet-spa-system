using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync(int page, int pageSize);
        Task<Product> GetProductByIdAsync(int id);
        Task<List<ProductCategory>> GetAllProductCategoriesAsync();
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        bool ProductExists(int id);
        Task<int> GetTotalProductCountAsync();
        Task DisableProductAsync(int id);
        Task EnsableProductAsync(int id);
        Task<List<Product>> GetSuggestedProductsAsync(int categoryId, int excludeProductId, int count);

        Task<List<Product>> GetActiveProductsAsync(int page, int pageSize);

        Task<int> CountActiveProductsAsync();
        Task<List<ProductWithRatingViewModel>> GetActiveProductsWithRatingAsync(int page, int pageSize);
        Task<Product?> GetProductWithReviewsByIdAsync(int productId);
<<<<<<< HEAD
        Task AddProductReviewAsync(int userId, int productId, int rating, string comment, bool isAnonymous);
        Task<List<Review>> GetRepliesForReviewAsync(int parentReviewId);
        Task AddReplyToReviewAsync(int parentReviewId, int userId, string content);
        Task<Review> GetLastReplyOfUserForParentAsync(int parentReviewId, int userId);
=======

        Task<List<ProductWithRatingViewModel>> GetActiveProductsWithRatingAsync(int page, int pageSize, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, string sort = null);
        Task<int> CountActiveProductsAsync(int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null);
>>>>>>> 05539e4eb903ebfb4b1fda55007f83236626e335
    }
}
