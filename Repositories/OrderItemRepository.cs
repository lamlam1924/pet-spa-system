using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly PetDataShopContext _context;

        public OrderItemRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
            => _context.OrderItems.Where(oi => oi.OrderId == orderId).ToList();

        public void AddOrderItem(OrderItem item)
        {
            _context.OrderItems.Add(item);
            _context.SaveChanges();
        }

        public void UpdateOrderItem(OrderItem item)
        {
            _context.OrderItems.Update(item);
            _context.SaveChanges();
        }

        public void DeleteOrderItem(int id)
        {
            var item = _context.OrderItems.FirstOrDefault(i => i.OrderItemId == id);
            if (item != null)
            {
                _context.OrderItems.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}
