using GestionClick.Api.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GestionClick.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageAsync(ChatMessage message, string userName)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, userName);
        }
        public async Task ChatNotificationAsync(string message, int receiverUserId, int senderUserId)
        {
            await Clients.All.SendAsync("ReceiveChatNotification", message, receiverUserId, senderUserId);
        }
    }
}
