using System.Collections.Generic;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _itemRepo;

        public OrderItemService(IOrderItemRepository itemRepo)
        {
            _itemRepo = itemRepo;
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
            => _itemRepo.GetOrderItemsByOrderId(orderId);

        public void AddOrderItem(OrderItem item)
            => _itemRepo.AddOrderItem(item);

        public void UpdateOrderItem(OrderItem item)
            => _itemRepo.UpdateOrderItem(item);

        public void DeleteOrderItem(int id)
            => _itemRepo.DeleteOrderItem(id);
    }
}
