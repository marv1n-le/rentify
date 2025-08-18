using Rentify.BusinessObjects.Entities.Base;
using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.Entities;

public class ChatMessage : BaseEntity
{
    public string ChatRoomId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Text;
    public bool IsEdited { get; set; } = false;
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedByUserId { get; set; }

    // Navigation properties
    public virtual ChatRoom? ChatRoom { get; set; }
    public virtual User? Sender { get; set; }
    public virtual User? DeletedByUser { get; set; }
}