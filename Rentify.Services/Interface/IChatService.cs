using Rentify.BusinessObjects.DTO.ChatDto;
using Rentify.BusinessObjects.Enum;

namespace Rentify.Services.Interface;

public interface IChatService
{
    Task<ChatRoomDto> CreateChatRoomAsync(CreateChatRoomDto createDto);
    Task<ChatRoomDto?> GetChatRoomByIdAsync(string roomId);
    Task<List<ChatRoomDto>> GetUserChatRoomsAsync(string userEmail);
    Task<List<ChatRoomDto>> GetAllChatRoomsAsync(); // Admin only
    Task<bool> DeleteChatRoomAsync(string roomId, string adminEmail);

    // Message Management
    Task<ChatMessageDto> SendMessageAsync(SendMessageDto sendDto);
    Task<List<ChatMessageDto>> GetRoomMessagesAsync(string roomId, int page = 1, int pageSize = 50);
    Task<bool> DeleteMessageAsync(string messageId, string userEmail);
    Task<ChatMessageDto?> EditMessageAsync(string messageId, string newContent, string userEmail);

    // Participant Management
    Task<bool> JoinRoomAsync(string roomId, string userEmail);
    Task<bool> LeaveRoomAsync(string roomId, string userEmail);
    Task<List<ChatParticipantDto>> GetRoomParticipantsAsync(string roomId);
    Task<bool> UpdateParticipantRoleAsync(string roomId, string userEmail, ParticipantRole role, string adminEmail);

    // User Management
    Task<List<UserOnlineDto>> GetOnlineUsersAsync();
    Task<bool> SetUserOnlineStatusAsync(string userEmail, bool isOnline);
    Task<ChatRoomDto?> GetOrCreatePrivateRoomAsync(string userEmail1, string userEmail2);

    // Admin Functions
    Task<ChatStatsDto> GetChatStatsAsync();
    Task<List<ChatMessageDto>> SearchMessagesAsync(string query, string? roomId = null);
    Task<bool> BanUserFromRoomAsync(string roomId, string userEmail, string adminEmail);
}