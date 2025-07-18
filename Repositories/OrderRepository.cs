using System.Collections.Generic;
using System.Linq;
using pet_spa_system1.Models;
using Microsoft.EntityFrameworkCore;

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
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Status)
                .Include(o => o.User) // BỔ SUNG DÒNG NÀY
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Status)
                .Include(o => o.User) // BỔ SUNG DÒNG NÀY
                .FirstOrDefault(o => o.OrderId == orderId);
        }

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

        public List<Order> GetOrdersByUserIdPaged(int userId, int page, int pageSize, out int totalOrders)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Status)
                .Include(o => o.User)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate);

            totalOrders = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
