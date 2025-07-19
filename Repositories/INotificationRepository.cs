using pet_spa_system1.Models;

namespace pet_spa_system1.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task AddNotificationAsync(Notification notification);
        Task MarkAllAsReadAsync(int userId);
        Task DeleteAllAsync(int userId);
    }
}
