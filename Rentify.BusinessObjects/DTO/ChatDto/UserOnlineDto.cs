using System.ComponentModel.DataAnnotations;

namespace Rentify.BusinessObjects.DTO.ChatDto;

public class UserOnlineDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string FullName { get; set; } = string.Empty;
    
    public string? ProfilePicture { get; set; }
    
    public bool IsOnline { get; set; } = false;
    
    public DateTime LastSeen { get; set; }
    
    public string Role { get; set; } = string.Empty;
    
    public bool IsAdmin { get; set; } = false;
}

public class ChatStatsDto
{
    public int TotalRooms { get; set; }
    public int TotalMessages { get; set; }
    public int ActiveUsers { get; set; }
    public int OnlineUsers { get; set; }
    public int TodayMessages { get; set; }
    public int ThisWeekMessages { get; set; }
    public List<ChatRoomStatsDto> TopActiveRooms { get; set; } = new();
}

public class ChatRoomStatsDto
{
    public string RoomId { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public int MessageCount { get; set; }
    public int ParticipantCount { get; set; }
    public DateTime LastActivity { get; set; }
}
