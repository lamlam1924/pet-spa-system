
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace pet_spa_system1.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotificationToUser(int userId, string title, string message)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", title, message);
        }
    }
}
