using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace UserManagementApi.Hubs
{
    public class UserStatusHub : Hub
    {
        public async Task NotifyUserStatusChange(int userId, bool isOnline)
        {
            await Clients.All.SendAsync("ReceiveUserStatusChange", userId, isOnline);
        }
    }
}