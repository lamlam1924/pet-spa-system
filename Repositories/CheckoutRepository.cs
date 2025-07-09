using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly PetDataShopContext _context;

        public CheckoutRepository(PetDataShopContext context)
        {
            _context = context;
        }

        // Lấy giỏ hàng theo userId
        public async Task<List<Cart>> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        // Lấy danh sách phương thức thanh toán
        public async Task<List<PaymentMethod>> GetPaymentMethodsAsync()
        {
            return await _context.PaymentMethods.ToListAsync();
        }

        // Lấy thông tin người dùng theo ID
        public async Task<User> GetUserByIdAsync(int userId)
        {

            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public Task<User> GetUserByIdAsync()
        {
            throw new NotImplementedException();
        }
    }
}

