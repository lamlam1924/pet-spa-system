using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pet_spa_system1.Repo
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly PetDataShopContext _context;

        public NotificationRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var noti in notifications)
                noti.IsRead = true;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();

            _context.Notifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();
        }
    }
}
