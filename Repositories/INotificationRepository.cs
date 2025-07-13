using pet_spa_system1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pet_spa_system1.Repo
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task AddNotificationAsync(Notification notification);
        Task MarkAllAsReadAsync(int userId);
        Task DeleteAllAsync(int userId);
    }
}
