namespace Rentify.BusinessObjects.DTO.ChatDto;

public class ChatSearchResultDto
{
    public List<ChatMessageDto> Messages { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}