using pet_spa_system1.Models;
using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        public UserService(IUserRepository repo) { _userRepo = repo; }
        public User? GetUserInfo(int userId) => _userRepo.GetUserById(userId);
    }
}

