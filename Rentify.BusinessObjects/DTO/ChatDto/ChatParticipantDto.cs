using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.DTO.ChatDto;

public class ChatParticipantDto
{
    public string Id { get; set; } = string.Empty;
    public string ChatRoomId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? UserProfilePicture { get; set; }
    public ParticipantRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LastSeenAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsOnline { get; set; } = false;
    public bool CanSendMessage { get; set; } = true;
    public bool CanDeleteMessage { get; set; } = false;
}