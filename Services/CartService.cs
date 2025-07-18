using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using System;
using System.Threading.Tasks;

namespace pet_spa_system1.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;

        public CartService(ICartRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<Cart>> GetCartByUserIdAsync(int userId)
        {
            return await _repository.GetCartByUserIdAsync(userId);
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0.", nameof(quantity));
            await _repository.AddToCartAsync(userId, productId, quantity);
        }

        public async Task RemoveFromCartAsync(int cartId)
        {
            await _repository.RemoveFromCartAsync(cartId);
        }

        public async Task UpdateQuantityAsync(int cartId, int newQuantity)
        {
            if (newQuantity < 0) throw new ArgumentException("Số lượng không thể âm.", nameof(newQuantity));
            await _repository.UpdateQuantityAsync(cartId, newQuantity);
        }

        public async Task ClearCartAsync(int userId)
        {
            await _repository.ClearCartAsync(userId);
        }

        public async Task RemoveProductFromCart(int productId, int userId)
        {
            await _repository.RemoveProductFromCart(productId, userId);
        }

        public async Task<Cart> IncreaseQuantityAsync(int productId, int userId)
        {
            return await _repository.IncreaseQuantityAsync(productId, userId);
        }

        public async Task<Cart> DecreaseQuantityAsync(int productId, int userId)
        {
            return await _repository.DecreaseQuantityAsync(productId, userId);
        }

        public async Task<decimal> GetTotalAmountAsync(int userId)
        {
            return await _repository.GetTotalAmountAsync(userId);
        }
    }
}