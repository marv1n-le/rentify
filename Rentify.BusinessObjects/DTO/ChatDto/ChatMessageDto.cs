using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.DTO.ChatDto;

public class ChatMessageDto
{
    public string Id { get; set; } = string.Empty;
    public string ChatRoomId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderProfilePicture { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Text;
    public DateTime CreatedDate { get; set; }
    public bool IsEdited { get; set; } = false;
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool CanEdit { get; set; } = false;
    public bool CanDelete { get; set; } = false;
}