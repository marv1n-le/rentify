using Microsoft.AspNetCore.SignalR;
using Rentify.RazorWebApp.Pages.ChatPages;
using System.Collections.Concurrent;

namespace Rentify.RazorWebApp.Hubs;

public class ChatHub : Hub
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ChatModel _chatModel;

    public ChatHub(IHttpContextAccessor httpContextAccessor, ChatModel chatModel)
    {
        _httpContextAccessor = httpContextAccessor;
        _chatModel = chatModel;
    }   

    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        // Load previous messages
        var messages = _chatModel.GetRoomMessages(roomId);
        await Clients.Caller.SendAsync("LoadMessages", messages);
    }

    public async Task SendMessage(string roomId, string message)
    {
        var userId = _httpContextAccessor.HttpContext?.Request.Cookies["userId"];
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ??
                      _httpContextAccessor.HttpContext?.Request.Cookies["Email"] ?? "Anonymous";

        if (!string.IsNullOrEmpty(userId))
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var chatMessage = new ChatMessage
            {
                UserId = userId,
                UserName = userName,
                Message = message,
                Timestamp = timestamp,
                RoomId = roomId
            };

            // Save to in-memory storage
            _chatModel.SaveMessageToRoom(roomId, chatMessage);

            // Broadcast to all room participants
            await Clients.Group(roomId).SendAsync("ReceiveMessage", chatMessage);
        }
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    // Thêm vào ChatHub.cs
    private static readonly ConcurrentDictionary<string, string> _connectedUsers = new();

    public async Task RegisterUser(string userId, string userName)
    {
        _connectedUsers.TryAdd(userId, userName);
        await Clients.All.SendAsync("UpdateOnlineUsers", _connectedUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Lấy userId từ connection
        var userId = _connectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        if (userId != null)
        {
            _connectedUsers.TryRemove(userId, out _);
            await Clients.All.SendAsync("UpdateOnlineUsers", _connectedUsers);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public List<object> GetOnlineUsersExceptCurrent(string currentUserId)
    {
        return _connectedUsers
            .Where(u => u.Key != currentUserId)
            .Select(u => new { id = u.Key, name = u.Value })
            .ToList<object>();
    }
}