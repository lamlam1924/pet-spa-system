using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly PetDataShopContext _context;

        public ProductService(IProductRepository repository, PetDataShopContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _repository.GetProductByIdAsync(id);
        }

        public async Task<List<Product>> GetAllProductsAsync(int page, int pageSize)
        {
            return await _repository.GetAllProductsAsync(page, pageSize);
        }

        public async Task<List<ProductCategory>> GetAllProductCategoriesAsync()
        {
            return await _repository.GetAllProductCategoriesAsync();
        }


        public async Task<List<Product>> GetActiveProductsAsync(int page, int pageSize)
        {
            return await _repository.GetActiveProductsAsync(page, pageSize);
        }

        public async Task<int> CountActiveProductsAsync()
        {
            return await _repository.CountActiveProductsAsync();
        }


        public async Task CreateProductAsync(Product product)
        {
            await _repository.AddProductAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            if (product.Stock == 0)
            {
                product.IsActive = false;
                _context.Products.Update(product); // ✅ Cập nhật vào DbContext

            }
            else
            {
                _context.Products.Update(product); // ✅ Cập nhật vào DbContext
            }
            await _context.SaveChangesAsync(); // ✅ Lưu lại thay đổi
        }


        public async Task DeleteProductAsync(int id)
        {
            await _repository.DeleteProductAsync(id);
        }

        public async Task DisableProductAsync(int id)
        {
            await _repository.DisableProductAsync(id);
        }

        public async Task EnableProductAsync(int id)
        {
            await _repository.EnsableProductAsync(id);
        }

        public bool ProductExists(int id)
        {
            return _repository.ProductExists(id);
        }

        public async Task<(Product Product, List<Product> SuggestedProducts)> GetProductDetailWithSuggestionsAsync(int productID)
        {
            var product = await _repository.GetProductByIdAsync(productID);
            if (product == null)
            {
                return (null, new List<Product>());
            }

            var suggestedProducts = await _repository.GetSuggestedProductsAsync(product.CategoryId, product.ProductId, 30);

            return (product, suggestedProducts);
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            return await _repository.GetTotalProductCountAsync();
        }

        public async Task<List<ProductWithRatingViewModel>> GetActiveProductsWithRatingAsync(int page, int pageSize)
        {
            return await _repository.GetActiveProductsWithRatingAsync(page, pageSize);
        }

        public async Task<Product?> GetProductWithReviewsByIdAsync(int productId)
        {
            return await _repository.GetProductWithReviewsByIdAsync(productId);
        }
        public async Task<List<ProductWithRatingViewModel>> GetActiveProductsWithRatingAsync(int page, int pageSize, int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null, string sort = null)
        {
        return await _repository.GetActiveProductsWithRatingAsync(page, pageSize, categoryId, minPrice, maxPrice, sort);
        }
        public async Task<int> CountActiveProductsAsync(int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
        return await _repository.CountActiveProductsAsync(categoryId, minPrice, maxPrice);
        }
        public async Task AddProductReviewAsync(int userId, int productId, int rating, string comment, bool isAnonymous)
        {
            await _repository.AddProductReviewAsync(userId, productId, rating, comment, isAnonymous);
        }

        public async Task<List<Review>> GetRepliesForReviewAsync(int parentReviewId)
        {
            return await _repository.GetRepliesForReviewAsync(parentReviewId);
        }

        public async Task AddReplyToReviewAsync(int parentReviewId, int userId, string content)
        {
            await _repository.AddReplyToReviewAsync(parentReviewId, userId, content);
        }

        public async Task AddSystemReplyToReviewAsync(int parentReviewId, int userId, string content)
        {
            await _repository.AddSystemReplyToReviewAsync(parentReviewId, userId, content);
        }

        public async Task<Review> GetLastReplyOfUserForParentAsync(int parentReviewId, int userId)
        {
            return await _repository.GetLastReplyOfUserForParentAsync(parentReviewId, userId);
        }

        public async Task<Review> GetReviewByIdAsync(int reviewId)
        {
            return await _repository.GetReviewByIdAsync(reviewId);
        }
        
        public async Task<Review> GetAutoReplyForReviewAsync(int parentReviewId)
        {
            return await _repository.GetAutoReplyForReviewAsync(parentReviewId);
        }
        
        public async Task DeleteReplyAsync(int reviewId)
        {
            await _repository.DeleteReplyAsync(reviewId);
        }
    }
}

