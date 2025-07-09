using pet_spa_system1.Models;

public interface ICartService
{
    Task<List<Cart>> GetCartByUserIdAsync(int userId);
    Task AddToCartAsync(int userId, int productId, int quantity);
    Task RemoveFromCartAsync(int cartId);
    Task UpdateQuantityAsync(int cartId, int newQuantity);
    Task ClearCartAsync(int userId);
    Task AddToCartAsync(int productId, int quantity);

    Task RemoveProductFromCart(int productId, int userId);

    Task<Cart> IncreaseQuantityAsync(int productId, int userId);
    Task<Cart> DecreaseQuantityAsync(int productId, int userId);

    Task<decimal> GetTotalAmountAsync(int userId);
}
