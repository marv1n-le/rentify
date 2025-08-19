using System.ComponentModel.DataAnnotations;
using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.DTO.ChatDto;

public class CreateChatRoomDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public ChatRoomType Type { get; set; } = ChatRoomType.Private;
    
    [Required]
    [EmailAddress]
    public string CreatedByEmail { get; set; } = string.Empty;
    
    public List<string> ParticipantEmails { get; set; } = new();
}