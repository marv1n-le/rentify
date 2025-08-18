using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Collections.Concurrent;
using Rentify.Services.Interface;
using System.Threading.Tasks;

namespace Rentify.RazorWebApp.Pages.ChatPages
{
    [Authorize]
    public class ChatModel : PageModel
    {
        private readonly IUserService _userService;

        public ChatModel(IUserService userService)
        {
            _userService = userService;
        }

        // In-memory storage (static to persist across requests)
        private static readonly ConcurrentDictionary<string, List<ChatMessage>> _roomMessages = new();
        private static readonly ConcurrentDictionary<string, List<string>> _chatRooms = new();

        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<ChatMessage> ChatHistory { get; set; } = new List<ChatMessage>();

        public async Task OnGet()
        {
            // Get user info from cookies
            UserId = _userService.GetCurrentUserId();
            var user = await _userService.GetUserById(UserId);
            UserEmail = user.Email;
            UserName = user.FullName;

            // Load chat history from session
            LoadChatHistory();
        }

        public IActionResult OnPostCreateRoom(string othersUserId)
        {
            if (string.IsNullOrEmpty(UserId)) return Unauthorized();

            var roomId = CreateOrGetPrivateRoom(UserId, othersUserId);
            return new JsonResult(new { roomId });
        }

        public IActionResult OnPostGetRoomMessages(string roomId)
        {
            var messages = GetRoomMessages(roomId);
            return new JsonResult(messages);
        }

        public async Task<IActionResult> OnGetGetAllUsersExceptCurrent()
        {
            // Lấy danh sách user từ database (đã loại trừ user hiện tại)
            var users = await _userService.GetAllUsersExceptCurrent();

            // Thêm trạng thái phòng chat nếu cần
            var result = users.Select(u => new {
                u.Id,
                u.FullName,
                HasExistingRoom = _chatRooms.Any(r =>
                    r.Value.Contains(UserId) &&
                    r.Value.Contains(u.Id))
            });

            return new JsonResult(result);
        }

        public IActionResult OnPostSaveMessage(string roomId, string message)
        {
            if (string.IsNullOrEmpty(UserId)) return Unauthorized();

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var chatMessage = new ChatMessage
            {
                UserId = UserId,
                UserName = UserName,
                Message = message,
                Timestamp = timestamp,
                RoomId = roomId
            };

            SaveMessageToRoom(roomId, chatMessage);
            SaveChatMessage(chatMessage); // Also save to session

            return new JsonResult(new { success = true });
        }

        public IActionResult OnPostClearHistory()
        {
            HttpContext.Session.Remove("ChatHistory");
            return new JsonResult(new { success = true });
        }

        private string CreateOrGetPrivateRoom(string userId1, string userId2)
        {
            // Find existing room
            var existingRoom = _chatRooms.FirstOrDefault(r =>
                r.Value.Contains(userId1) &&
                r.Value.Contains(userId2) &&
                r.Value.Count == 2).Key;

            if (!string.IsNullOrEmpty(existingRoom)) return existingRoom;

            // Create new room
            var roomId = Guid.NewGuid().ToString();
            _chatRooms.TryAdd(roomId, new List<string> { userId1, userId2 });
            _roomMessages.TryAdd(roomId, new List<ChatMessage>());

            return roomId;
        }

        public List<ChatMessage> GetRoomMessages(string roomId)
        {
            if (_roomMessages.TryGetValue(roomId, out var messages))
            {
                return messages.TakeLast(100).ToList();
            }
            return new List<ChatMessage>();
        }

        public void SaveMessageToRoom(string roomId, ChatMessage message)
        {
            if (_roomMessages.TryGetValue(roomId, out var messages))
            {
                messages.Add(message);

                // Keep only last 100 messages per room
                if (messages.Count > 100)
                {
                    messages.RemoveAt(0);
                }
            }
        }

        public void LoadChatHistory()
        {
            var historyJson = HttpContext.Session.GetString("ChatHistory");
            if (!string.IsNullOrEmpty(historyJson))
            {
                try
                {
                    ChatHistory = JsonSerializer.Deserialize<List<ChatMessage>>(historyJson) ?? new List<ChatMessage>();
                }
                catch
                {
                    ChatHistory = new List<ChatMessage>();
                }
            }
        }

        public void SaveChatMessage(ChatMessage message)
        {
            LoadChatHistory();
            ChatHistory.Add(message);

            // Keep only last 100 messages
            if (ChatHistory.Count > 100)
            {
                ChatHistory = ChatHistory.TakeLast(100).ToList();
            }

            var historyJson = JsonSerializer.Serialize(ChatHistory);
            HttpContext.Session.SetString("ChatHistory", historyJson);
        }
    }

    public class ChatMessage
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public string RoomId { get; set; } = string.Empty;
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}