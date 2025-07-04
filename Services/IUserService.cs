using pet_spa_system1.Models;

namespace pet_spa_system1.Services;

public interface IUserService
{
    Task<User?> GetUserByEmail(string email);
    Task<User?> AuthenticateAsync(string email, string password);
    Task<User?> RegisterAsync(string email, string userName, string plainPassword);
    Task<User?> RegisterByGoogle(string email, string name);
    User? GetUserInfo(int userId);
    List<User> GetAllStaff();
}