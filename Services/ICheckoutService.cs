using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface ICheckoutService
    {
        // Get payment methods
        Task<List<PaymentMethod>> GetPaymentMethodsAsync();
        // Get cart by user id
        Task<List<Cart>> GetCartByUserIdAsync(int userId);
        // Get discount code by code
        //Task<DiscountCode> GetDiscountCodeByCodeAsync(string code);
        // Get user by id
        Task<User> GetUserByIdAsync(int userId);
        // Process checkout
        Task<bool> ProcessCheckoutAsync(int userId, int paymentMethodId, string discountCode = null);

        Task<Order> GetOrderByIdAsync(string orderId);
        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task AddOrderItemsAsync(List<OrderItem> orderItems);
    }
}
