using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
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
            await _repository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _repository.DeleteProductAsync(id);
        }

        public async Task DisableProductAsync(int id)
        {
            await _repository.DisableProductAsync(id);
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
    }
}
