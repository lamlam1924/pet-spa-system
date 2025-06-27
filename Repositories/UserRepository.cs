using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;


namespace pet_spa_system1.Repositories

{
<<<<<<< HEAD
    public class UserRepository : GenericRepository<User>
=======
    public class UserRepository

          : GenericRepository<User>
>>>>>>> my-code
    {
        private readonly PetDataShopContext _context;
        public UserRepository(PetDataShopContext context) : base(context)
        {
            _context = context;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

    }
}
