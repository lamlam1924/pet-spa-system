using System.Collections.Generic;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public interface IOrderRepository
    {
        List<Order> GetOrdersByUserId(int userId);
        Order GetOrderById(int id);
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int id);
        List<Order> GetOrdersByUserIdPaged(int userId, int page, int pageSize, out int totalOrders);
        List<Order> GetAllOrders();
        List<Order> GetAllOrdersPaged(int page, int pageSize, out int totalOrders);
        List<Order> GetOrdersByStatusPaged(string status, int page, int pageSize, out int totalOrders);
    }
}
