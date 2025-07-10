using pet_spa_system1.Models;
using System.Threading.Tasks;

namespace pet_spa_system1.Services
{
    public interface IStaffService
    {
        Task<User> GetCurrentStaffProfileAsync();
        Task<User> SaveStaffProfileChangesAsync(User user);
    }
}