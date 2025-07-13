using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PetDataShopContext _context;

        public OrderRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public List<Order> GetOrdersByUserId(int userId)
            => _context.Orders.Where(o => o.UserId == userId).ToList();

        public Order GetOrderById(int id)
            => _context.Orders.FirstOrDefault(o => o.OrderId == id);

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void DeleteOrder(int id)
        {
            var order = GetOrderById(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }
    }
}
