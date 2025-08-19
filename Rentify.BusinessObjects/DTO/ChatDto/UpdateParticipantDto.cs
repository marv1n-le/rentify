using System.ComponentModel.DataAnnotations;
using Rentify.BusinessObjects.Enum;

namespace Rentify.BusinessObjects.DTO.ChatDto;

public class UpdateParticipantDto
{
    [Required]
    public string RoomId { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string UserEmail { get; set; } = string.Empty;
    
    public ParticipantRole Role { get; set; }
    
    public bool CanSendMessage { get; set; } = true;
    
    public bool CanDeleteMessage { get; set; } = false;
}