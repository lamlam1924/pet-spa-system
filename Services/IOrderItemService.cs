using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface IOrderItemService
    {
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
        void AddOrderItem(OrderItem item);
        void UpdateOrderItem(OrderItem item);
        void DeleteOrderItem(int id);
    }
}
