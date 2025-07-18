using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public interface IOrderStatusRepository
    {
        StatusOrder GetById(int id);
    }
}
