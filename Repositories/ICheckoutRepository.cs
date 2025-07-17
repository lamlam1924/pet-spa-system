using pet_spa_system1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pet_spa_system1.Repositories
{
    public interface ICheckoutRepository
    {
        // Get payment methods
        Task<List<PaymentMethod>> GetPaymentMethodsAsync();

        // Get cart by user ID
        Task<List<Cart>> GetCartByUserIdAsync(int userId);

        // Get discount code by code (khôi phục nếu cần)
        //Task<DiscountCode> GetDiscountCodeByCodeAsync(string code);

        // Get user by ID (giữ một phiên bản duy nhất, xóa phiên bản thừa)
        Task<User> GetUserByIdAsync(int userId);

        // Get order by ID (dùng cho VNPay với vnp_TxnRef)
        Task<Order> GetOrderByIdAsync(int orderId);

        // Create a new order
        Task<Order> CreateOrderAsync(Order order);

        // Update an existing order
        Task UpdateOrderAsync(Order order);

        // Get order items by order ID
        Task AddOrderItemsAsync(List<OrderItem> orderItems);
    }
}