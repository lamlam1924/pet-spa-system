using pet_spa_system1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pet_spa_system1.Repositories
{
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly PetDataShopContext _context;

        public CheckoutRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return await _context.PaymentMethods.ToListAsync();
        }

        public async Task<List<Cart>> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToListAsync();
        }

        //public async Task<DiscountCode> GetDiscountCodeByCodeAsync(string code)
        //{
        //    return await _context.DiscountCodes
        //        .FirstOrDefaultAsync(dc => dc.Code == code);
        //}

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        //public async Task<Order> GetOrderByIdAsync(string orderId)
        //{
        //    return await _context.Orders
        //        .FirstOrDefaultAsync(o => o.OrderId == orderId);
        //}

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}