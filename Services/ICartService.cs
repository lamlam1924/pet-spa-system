using pet_spa_system1.Models;

public interface ICartService
{
    Task<List<Cart>> GetCartByUserIdAsync(int userId);
    Task AddToCartAsync(Cart cart);
    Task RemoveFromCartAsync(int cartId);
    Task UpdateQuantityAsync(int cartId, int newQuantity);
    Task ClearCartAsync(int userId);
}
