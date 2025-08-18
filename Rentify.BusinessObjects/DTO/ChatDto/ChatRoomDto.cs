using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.DTO.ChatDto;

public class ChatRoomDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ChatRoomType Type { get; set; }
    public string CreatedByEmail { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime LastActivity { get; set; }
    public int ParticipantCount { get; set; }
    public int UnreadCount { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageTime { get; set; }
    public bool IsActive { get; set; } = true;
    public List<ChatParticipantDto> Participants { get; set; } = new();
}