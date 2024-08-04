using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using SignalRAssignment_ASM3.Models;
using System.Threading.Tasks;

namespace SignalRAssignment_ASM3.Hubs
{
        public class PostHub<T> : Hub
    {
        public async Task SendMessage(AppUser user, T message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendMessageToCaller(AppUser user, T message)
        {
            return Clients.Caller.SendAsync("ReceiveMessage", user, message);
        }

        public Task SendMessageToGroup(string group,  AppUser user, T message)
        {
            return Clients.Group(group).SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendPostUpdate(T post)
        {
            await Clients.All.SendAsync("ReceivePostUpdate", post);
        }

        public Task SendMessageToUser(AppUser user, T message)
        {
            return Clients.User(user.UserId.ToString()).SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR User");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR User");
            await base.OnDisconnectedAsync(exception);
        }

    }
}
