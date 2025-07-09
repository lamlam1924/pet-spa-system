using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;

        public CartService(ICartRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Cart>> GetCartByUserIdAsync(int userId)
        {
            return _repository.GetCartByUserIdAsync(userId);
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            await _repository.AddToCartAsync(userId, productId, quantity);
        }

        public async Task AddToCartAsync(int productId, int quantity)
        {
            await _repository.AddToCartAsync(1, productId, quantity);
        }

        public Task RemoveFromCartAsync(int cartId)
        {
            return _repository.RemoveFromCartAsync(cartId);
        }

        public Task UpdateQuantityAsync(int cartId, int newQuantity)
        {
            return _repository.UpdateQuantityAsync(cartId, newQuantity);
        }

        public Task ClearCartAsync(int userId)
        {
            return _repository.ClearCartAsync(userId);
        }
        public Task RemoveProductFromCart(int productId,int userId)
        {
            return _repository.RemoveProductFromCart(productId,userId);

        }

        public Task<Cart> IncreaseQuantityAsync(int productId, int userId)
        {
            return _repository.IncreaseQuantityAsync(productId, userId);
        }
        public Task<Cart> DecreaseQuantityAsync(int productId, int userId)
        {
            return _repository.DecreaseQuantityAsync(productId, userId);

        }
        public Task<decimal> GetTotalAmountAsync(int userId)
        {
            return _repository.GetTotalAmountAsync(userId);
        }
    }
}
