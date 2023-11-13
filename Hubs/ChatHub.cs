using Microsoft.AspNetCore.SignalR;

namespace UNITEE_BACKEND.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task SendMessageToRoom(string roomName, string user, string message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
        }
    }
}
