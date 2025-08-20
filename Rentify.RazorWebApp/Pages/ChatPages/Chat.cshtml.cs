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
        private readonly IChatService _chatService; 

        public ChatModel(IUserService userService, IChatService chatService)
        {
            _userService = userService;
            _chatService = chatService; 
        }
        
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public async Task OnGet()
        {
            await LoadCurrentUserInfo();
        }

        public async Task<IActionResult> OnPostCreateRoom(string othersUserId)
        {
            await LoadCurrentUserInfo();
            if (string.IsNullOrEmpty(UserId)) return Unauthorized();

            var otherUser = await _userService.GetUserById(othersUserId);

            // SỬ DỤNG CHATSERVICE ĐỂ LƯU XUỐNG DATABASE
            var room = await _chatService.GetOrCreatePrivateRoomAsync(UserEmail, otherUser.Email);

            return new JsonResult(new { roomId = room.Id });
        }

        public async Task<IActionResult> OnGetGetRoomMessages(string roomId)
        {
            var messages = await _chatService.GetRoomMessagesAsync(roomId);
    
            var result = messages.Select(m => new {
                userId = m.SenderId,
                userEmail = m.SenderEmail,  
                userName = m.SenderName,
                message = m.Content,       
                content = m.Content,       
                timestamp = m.CreatedDate.ToString("HH:mm:ss")
            });
    
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnGetGetAllUsersExceptCurrent()
        {
            await LoadCurrentUserInfo();
            var users = await _userService.GetAllUsersExceptCurrent();

            var result = users.Select(u => new
            {
                u.Id,
                u.FullName,
                u.ProfilePicture 
            });

            return new JsonResult(result);
        }

        private async Task LoadCurrentUserInfo()
        {
            UserId = _userService.GetCurrentUserId();
            var user = await _userService.GetUserById(UserId);
            UserEmail = user.Email;
            UserName = user.FullName;
        }
    }
}