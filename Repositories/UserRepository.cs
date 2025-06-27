
using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PetDataShopContext _context;
        public UserRepository(PetDataShopContext context) { _context = context; }

        public User? GetUserById(int userId)
            => _context.Users.FirstOrDefault(u => u.UserId == userId);
    }

}

