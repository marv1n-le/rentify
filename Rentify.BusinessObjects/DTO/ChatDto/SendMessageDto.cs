using System.ComponentModel.DataAnnotations;
using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.DTO.ChatDto;

public class SendMessageDto
{
    [Required]
    public string RoomId { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string SenderEmail { get; set; } = string.Empty;
    
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
    
    public MessageType Type { get; set; } = MessageType.Text;
}