using pet_spa_system1.Models;
using System.Threading.Tasks;

namespace pet_spa_system1.Repositories
{
    public interface IStaffRepository
    {
        Task<User> GetStaffProfileByIdAsync(int userId);
        Task<User> UpdateStaffProfileAsync(User user);
        Task<bool> StaffExistsAsync(int userId);
    }
}