using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.DTO.ChatDto;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;
using Rentify.Services.Hub;
using Rentify.Services.Interface;

namespace Rentify.Services.Service;

public class ChatService : IChatService
{
    private readonly RentifyDbContext _context;
    private readonly IMapper _mapper;
    private readonly Dictionary<string, DateTime> _onlineUsers = new();
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatService(RentifyDbContext context, IMapper mapper, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<ChatRoomDto> CreateChatRoomAsync(CreateChatRoomDto createDto)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == createDto.CreatedByEmail);
            if (user == null)
                throw new ArgumentException("User not found");

            var chatRoom = new ChatRoom
            {
                Id = Guid.NewGuid().ToString(),
                Name = createDto.Name,
                Description = createDto.Description,
                Type = createDto.Type,
                CreatedByUserId = user.Id!,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                LastActivity = DateTime.UtcNow
            };

            _context.ChatRooms.Add(chatRoom);

            // Add creator as owner
            var participant = new ChatParticipant
            {
                Id = Guid.NewGuid().ToString(),
                ChatRoomId = chatRoom.Id,
                UserId = user.Id!,
                Role = ParticipantRole.Owner,
                JoinedAt = DateTime.UtcNow,
                IsActive = true,
                CanSendMessage = true,
                CanDeleteMessage = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatParticipants.Add(participant);
            await _context.SaveChangesAsync();

            return _mapper.Map<ChatRoomDto>(chatRoom);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating chat room: {ex.Message}");
        }
    }

    public async Task<ChatRoomDto?> GetChatRoomByIdAsync(string roomId)
    {
        var room = await _context.ChatRooms
            .Include(r => r.Participants)
            .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(r => r.Id == roomId && r.IsActive);

        return room != null ? _mapper.Map<ChatRoomDto>(room) : null;
    }

    public async Task<List<ChatRoomDto>> GetUserChatRoomsAsync(string userEmail)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return new List<ChatRoomDto>();

        var rooms = await _context.ChatRooms
            .Include(r => r.Participants)
            .ThenInclude(p => p.User)
            .Where(r => r.Participants.Any(p => p.UserId == user.Id && p.IsActive) && r.IsActive)
            .OrderByDescending(r => r.LastActivity)
            .ToListAsync();

        return _mapper.Map<List<ChatRoomDto>>(rooms);
    }

    public async Task<List<ChatRoomDto>> GetAllChatRoomsAsync()
    {
        var rooms = await _context.ChatRooms
            .Include(r => r.Participants)
            .ThenInclude(p => p.User)
            .Include(r => r.CreatedByUser)
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.LastActivity)
            .ToListAsync();

        return _mapper.Map<List<ChatRoomDto>>(rooms);
    }

    public async Task<ChatMessageDto> SendMessageAsync(SendMessageDto sendDto)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == sendDto.SenderEmail);
            if (user == null)
                throw new ArgumentException("User not found");

            var participant = await _context.ChatParticipants
                .FirstOrDefaultAsync(p => p.ChatRoomId == sendDto.RoomId && p.UserId == user.Id && p.IsActive);

            if (participant == null || !participant.CanSendMessage)
                throw new UnauthorizedAccessException("User cannot send messages in this room");

            var message = new ChatMessage
            {
                Id = Guid.NewGuid().ToString(),
                ChatRoomId = sendDto.RoomId,
                SenderId = user.Id!,
                Content = sendDto.Content,
                Type = sendDto.Type,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);

            // Update room last activity
            var room = await _context.ChatRooms.FindAsync(sendDto.RoomId);
            if (room != null)
            {
                room.LastActivity = DateTime.UtcNow;
                room.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            // Load message with sender info
            var savedMessage = await _context.ChatMessages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == message.Id);

            await _hubContext.Clients.Group(sendDto.RoomId)
                .SendAsync("ReceiveMessage", 
                    savedMessage.Sender.Email,
                    savedMessage.Sender.FullName,
                    savedMessage.Content,
                    savedMessage.CreatedAt);
            
            return _mapper.Map<ChatMessageDto>(savedMessage);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error sending message: {ex.Message}");
        }
    }

    public async Task<List<ChatMessageDto>> GetRoomMessagesAsync(string roomId, int page = 1, int pageSize = 50)
    {
        var messages = await _context.ChatMessages
            .Include(m => m.Sender)
            .Where(m => m.ChatRoomId == roomId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return _mapper.Map<List<ChatMessageDto>>(messages.OrderBy(m => m.CreatedAt).ToList());
    }

    public async Task<bool> JoinRoomAsync(string roomId, string userEmail)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return false;

            var existingParticipant = await _context.ChatParticipants
                .FirstOrDefaultAsync(p => p.ChatRoomId == roomId && p.UserId == user.Id);

            if (existingParticipant != null)
            {
                existingParticipant.IsActive = true;
                existingParticipant.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var participant = new ChatParticipant
                {
                    Id = Guid.NewGuid().ToString(),
                    ChatRoomId = roomId,
                    UserId = user.Id!,
                    Role = ParticipantRole.Member,
                    JoinedAt = DateTime.UtcNow,
                    IsActive = true,
                    CanSendMessage = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ChatParticipants.Add(participant);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ChatRoomDto?> GetOrCreatePrivateRoomAsync(string userEmail1, string userEmail2)
    {
        var user1 = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail1);
        var user2 = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail2);

        if (user1 == null || user2 == null) return null;

        // Check if private room already exists
        var existingRoom = await _context.ChatRooms
            .Include(r => r.Participants)
            .Where(r => r.Type == ChatRoomType.Private && r.IsActive)
            .FirstOrDefaultAsync(r => r.Participants.Count == 2 &&
                r.Participants.Any(p => p.UserId == user1.Id && p.IsActive) &&
                r.Participants.Any(p => p.UserId == user2.Id && p.IsActive));

        if (existingRoom != null)
        {
            return _mapper.Map<ChatRoomDto>(existingRoom);
        }

        // Create new private room
        var createDto = new CreateChatRoomDto
        {
            Name = $"Private Chat - {user1.FullName} & {user2.FullName}",
            Type = ChatRoomType.Private,
            CreatedByEmail = userEmail1
        };

        var newRoom = await CreateChatRoomAsync(createDto);
        await JoinRoomAsync(newRoom.Id, userEmail2);

        return newRoom;
    }

    public async Task<List<UserOnlineDto>> GetOnlineUsersAsync()
    {
        var onlineEmails = _onlineUsers.Where(u => DateTime.UtcNow.Subtract(u.Value).TotalMinutes < 5)
            .Select(u => u.Key).ToList();

        var users = await _context.Users
            .Where(u => onlineEmails.Contains(u.Email!))
            .Select(u => new UserOnlineDto
            {
                Email = u.Email!,
                FullName = u.FullName!,
                ProfilePicture = u.ProfilePicture,
                IsOnline = true,
                LastSeen = _onlineUsers.ContainsKey(u.Email!) ? _onlineUsers[u.Email!] : DateTime.UtcNow
            })
            .ToListAsync();

        return users;
    }

    public async Task<bool> SetUserOnlineStatusAsync(string userEmail, bool isOnline)
    {
        if (isOnline)
        {
            _onlineUsers[userEmail] = DateTime.UtcNow;
        }
        else
        {
            _onlineUsers.Remove(userEmail);
        }

        return await Task.FromResult(true);
    }

    public async Task<ChatStatsDto> GetChatStatsAsync()
    {
        var totalRooms = await _context.ChatRooms.CountAsync(r => r.IsActive);
        var totalMessages = await _context.ChatMessages.CountAsync(m => !m.IsDeleted);
        var activeUsers = await _context.ChatParticipants.CountAsync(p => p.IsActive);
        var onlineUsers = _onlineUsers.Count(u => DateTime.UtcNow.Subtract(u.Value).TotalMinutes < 5);

        return new ChatStatsDto
        {
            TotalRooms = totalRooms,
            TotalMessages = totalMessages,
            ActiveUsers = activeUsers,
            OnlineUsers = onlineUsers
        };
    }

    // Additional methods implementation...
    public async Task<bool> DeleteChatRoomAsync(string roomId, string adminEmail) => await Task.FromResult(false);
    public async Task<bool> DeleteMessageAsync(string messageId, string userEmail) => await Task.FromResult(false);
    public async Task<ChatMessageDto?> EditMessageAsync(string messageId, string newContent, string userEmail) => await Task.FromResult<ChatMessageDto?>(null);
    public async Task<bool> LeaveRoomAsync(string roomId, string userEmail) => await Task.FromResult(false);
    public async Task<List<ChatParticipantDto>> GetRoomParticipantsAsync(string roomId) => await Task.FromResult(new List<ChatParticipantDto>());
    public async Task<bool> UpdateParticipantRoleAsync(string roomId, string userEmail, ParticipantRole role, string adminEmail) => await Task.FromResult(false);
    public async Task<List<ChatMessageDto>> SearchMessagesAsync(string query, string? roomId = null) => await Task.FromResult(new List<ChatMessageDto>());
    public async Task<bool> BanUserFromRoomAsync(string roomId, string userEmail, string adminEmail) => await Task.FromResult(false);
}
