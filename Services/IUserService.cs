using pet_spa_system1.Models;

namespace pet_spa_system1.Services;

public interface IUserService
{
    User? GetUserInfo(int userId);
}