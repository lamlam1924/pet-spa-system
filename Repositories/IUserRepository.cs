using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task SaveChangesAsync();
    Task<Role?> GetRoleByIdAsync(int roleId);
    User? GetUserById(int userId);
    Task AddAsync(User user);
    Task<List<User>> GetActiveUsersAsync(string? search = null, string? sort = null);
    Task<List<User>> GetDeletedUsersAsync();
    Task<List<Role>> GetActiveRolesAsync();
    Task UpdateAsync(User user);
    Task<User?> GetByIdAsync(int userId);
}