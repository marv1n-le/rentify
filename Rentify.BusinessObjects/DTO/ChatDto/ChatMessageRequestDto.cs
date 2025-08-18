using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.DTO.ChatDto
{
    public class ChatMessageRequestDto
    {
        public string Message { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string ChatRoomId { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Text;
    }
}
