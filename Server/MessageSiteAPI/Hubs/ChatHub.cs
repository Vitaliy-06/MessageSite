using Microsoft.AspNetCore.SignalR;

namespace MessageSiteAPI.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string room, object data)
        {
            await Clients.Group(room).SendAsync("ReceiveMessage", data);
        }
        public async Task JoinRoom(string room)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
        }

        public async Task LeaveRoom(string room)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        }
    }
}
