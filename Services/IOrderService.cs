using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    public interface IOrderService
    {
        List<Order> GetOrdersByUserId(int userId);
        Order GetOrderById(int id);
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int id);
    }
}
