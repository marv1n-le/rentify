using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.DTO.ChatDto;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;
using System.Text;
using System.Text.Json;

namespace Rentify.RazorWebApp.Pages.ChatPages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        public string BaseUrl { get; private set; }

        public IndexModel(IConfiguration config, IUserService userService, IHttpClientFactory httpClient, IHttpContextAccessor contextAccessor)
        {
            _config = config;
            _userService = userService;
            _httpClient = httpClient;
            BaseUrl = _config.GetValue<string>("BaseUrl") ?? "https://api.mamafit.studio";
            _contextAccessor = contextAccessor;
        }

        [BindProperty]
        public List<ChatMessageResponseDto>? ChatMessageList { get; set; } = default!;

        [BindProperty]
        public List<User> UserList { get; set; } = default!;

        [BindProperty]
        public User CurrentUser { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            var currentUserId = GetCurrentUserId();
            var user = await _userService.GetUserById(currentUserId);
            CurrentUser = user;

            await OnGetUserList();
            return Page();
        }

        public async Task<IActionResult> OnGetUserList()
        {
            var userId = GetCurrentUserId();
            var listUser = await _userService.GetAllUsers();
            listUser = listUser.Where(x => x.Id != userId);

            UserList = listUser.ToList();
            return Page();
        }

        public async Task<IActionResult> OnGetChatHistory(string roomId, int index = 1, int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            var httpClient = _httpClient.CreateClient();

            httpClient.Timeout = TimeSpan.FromSeconds(30);
            var getChatHistoryApi = $"{BaseUrl}/api/Chat/rooms/{roomId}/messages?index={index}&pageSize={pageSize}";

            var response = await httpClient.GetAsync(getChatHistoryApi);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var responseModel = JsonSerializer.Deserialize<ResponseModel<List<ChatMessageResponseDto>>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (responseModel.StatusCode == 200)
                {
                    return new JsonResult(new { success = true, chatMessageList = responseModel.Data });
                }
                return BadRequest("Error when taking message");
            }
            return BadRequest("Error when taking message");
        }

        public async Task<IActionResult> OnPostCreateChatRoom([FromForm]string userId)
        {
            var currentUserId = GetCurrentUserId();
            var currentUser = _userService.GetUserById(currentUserId);

            var httpClient = _httpClient.CreateClient();

            var request = new ChatRoomCreateWithUserIdDto
            {
                UserId1 = currentUserId,
                UserId2 = userId,
            };

            var serializeRequest = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(serializeRequest, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{BaseUrl}/api/Chat/rooms", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var responseModel = JsonSerializer.Deserialize<ResponseModel<ChatRoomResponseDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (responseModel != null && responseModel.StatusCode == 200 && responseModel.Data != null)
                {
                    return new JsonResult(new { success = true, roomId = responseModel.Data.Id });
                }
                return BadRequest("Error creating chat room: Invalid response from API.");
            }
            return BadRequest("Error creating chat room: Request failed.");
        }

        private string GetCurrentUserId()
        {
            var userId = _contextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out var value) ? value.ToString() : null;
            return userId;
        }

        public class ResponseModel<T>
        {
            public T? Data { get; set; }
            public string? Message { get; set; }
            public int StatusCode { get; set; }
            public string Code { get; set; }

            public ResponseModel(int statusCode, string code, T? data, string? message = null)
            {
                this.StatusCode = statusCode;
                this.Code = code;
                this.Data = data;
                this.Message = message;
            }

            public ResponseModel(int statusCode, string code, string? message)
            {
                this.StatusCode = statusCode;
                this.Code = code;
                this.Message = message;
            }
        }
    }
}
