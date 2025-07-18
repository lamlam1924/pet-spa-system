using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;
using System.Collections.Generic;

namespace pet_spa_system1.Services
{
    public interface IOrderService
    {
        List<Order> GetOrdersByUserId(int userId);
        Order GetOrderById(int id);
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int id);
        List<OrderViewModel> GetOrdersByUserId(int? userId);
        OrderViewModel GetOrderDetail(int orderId);
        bool CancelOrder(int orderId, int? userId);
        List<OrderViewModel> GetOrdersByUserIdPaged(int? userId, int page, int pageSize, out int totalOrders);
    }
}
