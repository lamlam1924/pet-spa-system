using pet_spa_system1.Models;

namespace pet_spa_system1.Services;

public interface IUserService
{
    Task<User?> GetUserByEmail(string email);
    Task<User?> AuthenticateAsync(string email, string password);
    Task<User?> RegisterAsync(string email, string userName, string plainPassword);
    Task<User?> RegisterByGoogle(string email, string name);
    
    User? GetUserInfo(int userId);

    Task<List<User>> GetActiveUsersAsync(string? search = null, string? sort = null);
    Task<List<User>> GetDeletedUsersAsync();
    Task<List<Role>> GetActiveRolesAsync();
    Task<(bool Success, string? Message, string? DefaultPassword)> CreateUserAsync(User user);
    Task<(bool Success, string? Message)> EditUserAsync(User updated);
    Task<(bool Success, string? Message)> DeleteUserAsync(int id);
    Task<(bool Success, string? Message)> RestoreUserAsync(int id);
    Task<(bool Success, string? Message, string? NewPassword)> ResetPasswordAsync(int id);
    Task<User?> GetUserByIdAsync(int userId);
}