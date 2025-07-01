using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories;

public interface IUserRepository
{
    User? GetUserById(int userId);
}