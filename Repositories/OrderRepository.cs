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
                // Xóa các payment liên quan trước
                var payments = _context.Payments.Where(p => p.OrderId == id).ToList();
                if (payments.Any())
                {
                    _context.Payments.RemoveRange(payments);
                }
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

        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Status)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public List<Order> GetAllOrdersPaged(int page, int pageSize, out int totalOrders)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Status)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate);

            totalOrders = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<Order> GetOrdersByStatusPaged(string status, int page, int pageSize, out int totalOrders)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Status)
                .Include(o => o.User)
                .Where(o => o.Status.StatusName == status)
                .OrderByDescending(o => o.OrderDate);

            totalOrders = query.Count();
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
