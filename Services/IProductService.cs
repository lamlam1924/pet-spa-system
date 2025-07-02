using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetAllProductsAsync(int page, int pageSize);
        Task<List<ProductCategory>> GetAllProductCategoriesAsync();
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        bool ProductExists(int id);
        Task<(Product Product, List<Product> SuggestedProducts)> GetProductDetailWithSuggestionsAsync(int productID);
        
        Task<int> GetTotalProductCountAsync();
        Task DisableProductAsync(int id);

        Task<List<Product>> GetActiveProductsAsync(int page, int pageSize);
        Task<int> CountActiveProductsAsync();
    }
}
