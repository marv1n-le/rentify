// ChatHub.cs

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Rentify.BusinessObjects.DTO.ChatDto;
using Rentify.BusinessObjects.Enum;
using Rentify.Services.Interface;

namespace Rentify.Services.Hub
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IChatService _chatService;
        private static readonly Dictionary<string, string> _userConnections = new();

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            
            if (email != null)
            {
                _userConnections[email] = Context.ConnectionId;
                await _chatService.SetUserOnlineStatusAsync(email, true);
                await Clients.All.SendAsync("UserOnline", email);
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            
            if (email != null && _userConnections.ContainsKey(email))
            {
                _userConnections.Remove(email);
                await _chatService.SetUserOnlineStatusAsync(email, false);
                await Clients.All.SendAsync("UserOffline", email);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(string roomId, string userEmail)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await _chatService.JoinRoomAsync(roomId, userEmail);
            await Clients.Group(roomId).SendAsync("UserJoined", userEmail);
        }

        public async Task SendMessage(string roomId, string senderEmail, string message)
        {
            var sendDto = new SendMessageDto
            {
                RoomId = roomId,
                SenderEmail = senderEmail,
                Content = message,
                Type = MessageType.Text
            };

            await _chatService.SendMessageAsync(sendDto);
        }
    }
}