using System.ComponentModel.DataAnnotations;

namespace Rentify.BusinessObjects.DTO.ChatDto;

public class ChatSearchDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Query { get; set; } = string.Empty;
    
    public string? RoomId { get; set; }
    
    public string? SenderEmail { get; set; }
    
    public DateTime? FromDate { get; set; }
    
    public DateTime? ToDate { get; set; }
    
    public int Page { get; set; } = 1;
    
    public int PageSize { get; set; } = 20;
}