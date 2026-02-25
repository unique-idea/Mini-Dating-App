using Microsoft.AspNetCore.SignalR;
namespace Mini_Dating_App_BE.Hubs
{
    public class SystemHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }

    }
}
