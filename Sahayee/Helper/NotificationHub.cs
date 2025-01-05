using Microsoft.AspNetCore.SignalR;

namespace Sahayee.Helper
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", new { Type = "Info", Data = message });
        }
    }
}
