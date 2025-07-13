using System.Collections.Generic;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public List<Order> GetOrdersByUserId(int userId)
            => _orderRepo.GetOrdersByUserId(userId);

        public Order GetOrderById(int id)
            => _orderRepo.GetOrderById(id);

        public void AddOrder(Order order)
            => _orderRepo.AddOrder(order);

        public void UpdateOrder(Order order)
            => _orderRepo.UpdateOrder(order);

        public void DeleteOrder(int id)
            => _orderRepo.DeleteOrder(id);
    }
}
