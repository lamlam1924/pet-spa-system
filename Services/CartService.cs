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

        public Task AddToCartAsync(Cart cart)
        {
            return _repository.AddToCartAsync(cart);
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
    }
}
