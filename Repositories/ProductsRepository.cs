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
                .Include(p => p.ProductCategory)
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
                .Include(p => p.ProductCategory)
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
                    CategoryName = p.ProductCategory.Name,
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
                .FirstOrDefaultAsync(p => p.ProductId == productId);

        }
    }
}
