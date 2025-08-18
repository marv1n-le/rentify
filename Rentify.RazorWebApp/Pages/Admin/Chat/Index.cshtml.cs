using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Services.Interface;
using Rentify.BusinessObjects.DTO.ChatDto;
using System.Security.Claims;
using System.Linq;

namespace Rentify.RazorWebApp.Pages.Admin.Chat
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IChatService _chatService;

        public IndexModel(IUserService userService, IChatService chatService)
        {
            _userService = userService;
            _chatService = chatService;
        }

        public string CurrentAdminEmail { get; set; } = string.Empty;
        public List<ChatRoomDto> AllRooms { get; set; } = new();
        public List<UserOnlineDto> OnlineUsers { get; set; } = new();

        public async Task OnGet()
        {
            // Lấy email admin hiện tại từ claims
            CurrentAdminEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

            // Tải danh sách phòng & user online
            AllRooms = await _chatService.GetAllChatRoomsAsync();
            OnlineUsers = await _chatService.GetOnlineUsersAsync();
        }

        // Lấy info phòng theo Id (dùng service hiện có)
        public async Task<IActionResult> OnGetRoomInfoAsync(string roomId)
        {
            var room = await _chatService.GetChatRoomByIdAsync(roomId);
            if (room == null) return new JsonResult(new { error = "Room not found" });

            return new JsonResult(new
            {
                name = room.Name,
                participants = room.Participants,
                createdAt = room.CreatedDate.ToString("dd/MM/yyyy HH:mm")
            });
        }

        // Lịch sử tin nhắn (dùng GetRoomMessagesAsync đang có)
        public async Task<IActionResult> OnGetRoomMessagesAsync(string roomId, int page = 1)
        {
            var messages = await _chatService.GetRoomMessagesAsync(roomId, page, 100);
            return new JsonResult(new { messages });
        }

        // Tạo (hoặc lấy) private room giữa admin hiện tại và 1 user theo email
        public async Task<IActionResult> OnPostCreateRoomWithUserAsync([FromBody] CreatePrivateRoomInput input)
        {
            CurrentAdminEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(CurrentAdminEmail) || string.IsNullOrWhiteSpace(input.OtherUserEmail))
                return new JsonResult(new { success = false, error = "Invalid emails" });

            var room = await _chatService.GetOrCreatePrivateRoomAsync(CurrentAdminEmail, input.OtherUserEmail);
            if (room == null) return new JsonResult(new { success = false, error = "Cannot create/get room" });

            return new JsonResult(new { success = true, roomId = room.Id, roomName = room.Name });
        }

        // Admin gửi tin nhắn vào 1 room
        public async Task<IActionResult> OnPostSendAdminMessage([FromBody] AdminMessageDto model)
        {
            CurrentAdminEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(CurrentAdminEmail)) 
                return new JsonResult(new { success = false, error = "No admin email" });

            var send = new SendMessageDto
            {
                RoomId = model.RoomId,
                SenderEmail = CurrentAdminEmail,
                Content = model.Message
            };
            var result = await _chatService.SendMessageAsync(send);
            return new JsonResult(new { success = true, message = result });
        }

        public class CreatePrivateRoomInput
        {
            public string OtherUserEmail { get; set; } = string.Empty;
        }

        public class AdminMessageDto
        {
            public string RoomId { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }
    }
}
