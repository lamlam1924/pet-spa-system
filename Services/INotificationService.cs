using pet_spa_system1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pet_spa_system1.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetByUserIdAsync(int userId);
        Task AddAsync(Notification notification);
        Task MarkAllAsReadAsync(int userId);
        Task DeleteAllAsync(int userId);
        // void SendServiceCancelNotification(int appointmentId, int serviceId, string reason = "");
    }
}
