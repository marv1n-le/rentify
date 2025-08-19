using Rentify.BusinessObjects.Entities.Base;
using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.Entities;

public class ChatParticipant : BaseEntity
{
    public string ChatRoomId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public ParticipantRole Role { get; set; } = ParticipantRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSeenAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool CanSendMessage { get; set; } = true;
    public bool CanDeleteMessage { get; set; } = false;

    // Navigation properties
    public virtual ChatRoom? ChatRoom { get; set; }
    public virtual User? User { get; set; }
}
