using Rentify.BusinessObjects.Entities.Base;
using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.Entities;

public class ChatRoom : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ChatRoomType Type { get; set; } = ChatRoomType.Private;
    public string CreatedByUserId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User? CreatedByUser { get; set; }
    public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    public virtual ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
}
