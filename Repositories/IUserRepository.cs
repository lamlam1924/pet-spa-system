using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task SaveChangesAsync();
    Task<Role?> GetRoleByIdAsync(int roleId);
    User? GetUserById(int userId);
    Task AddAsync(User user);
    List<User> GetAllStaff();
}