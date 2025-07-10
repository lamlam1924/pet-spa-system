using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using System.Threading.Tasks;

namespace pet_spa_system1.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly PetDataShopContext _context;

        public StaffRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public async Task<User> GetStaffProfileByIdAsync(int userId)
        {
            return await _context.Users
                .Where(u => u.UserId == userId && u.IsActive == true)
                .FirstOrDefaultAsync();
        }

        public async Task<User> UpdateStaffProfileAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> StaffExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId && u.IsActive == true);
        }
    }
}