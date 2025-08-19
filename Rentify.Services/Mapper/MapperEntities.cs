using AutoMapper;
using Rentify.BusinessObjects.DTO.Inquiry;
using Rentify.BusinessObjects.DTO.ChatDto;
using Rentify.BusinessObjects.DTO.PostDto;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.DTO.ItemDto;

namespace Rentify.Services.Mapper
{
    public class MapperEntities : Profile
    {
        public MapperEntities()
        {
            CreateMap<Post, PostUpdateRequestDto>().ReverseMap();

            CreateMap<Item, ItemCreateDto>().ReverseMap();
            CreateMap<Item, ItemUpdateDto>().ReverseMap();

            CreateMap<ChatRoom, ChatRoomDto>()
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
                .ReverseMap();
            CreateMap<ChatMessage, ChatMessageDto>()
                .ForMember(dest => dest.SenderEmail, opt => opt.MapFrom(src => src.Sender.Email))
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
                .ForMember(dest => dest.SenderProfilePicture, opt => opt.MapFrom(src => src.Sender.ProfilePicture))
                .ForMember(dest => dest.CanEdit, opt => opt.MapFrom(src => src.Sender.Id == src.SenderId))
                .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => src.Sender.Id == src.SenderId && !src.IsDeleted))
                .ReverseMap();
            CreateMap<User, UserOnlineDto>()
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email ?? string.Empty))
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName ?? string.Empty))
                .ForMember(d => d.ProfilePicture, o => o.MapFrom(s => s.ProfilePicture))
                .ForMember(d => d.IsOnline, o => o.Ignore())
                .ForMember(d => d.LastSeen, o => o.Ignore())
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role != null ? s.Role.Name : string.Empty))
                .ForMember(d => d.IsAdmin, o => o.MapFrom(s => s.Role != null && s.Role.Name == "Admin"));

            CreateMap<ChatParticipant, ChatParticipantDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ChatRoomId, o => o.MapFrom(s => s.ChatRoomId))
                .ForMember(d => d.UserEmail, o => o.MapFrom(s => s.User != null ? (s.User.Email ?? string.Empty) : string.Empty))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? (s.User.FullName ?? string.Empty) : string.Empty))
                .ForMember(d => d.UserProfilePicture, o => o.MapFrom(s => s.User != null ? s.User.ProfilePicture : null))
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role))
                .ForMember(d => d.JoinedAt, o => o.MapFrom(s => s.JoinedAt))
                .ForMember(d => d.LastSeenAt, o => o.Ignore())
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive))
                .ForMember(d => d.IsOnline, o => o.Ignore())
                .ForMember(d => d.CanSendMessage, o => o.MapFrom(s => s.CanSendMessage))
                .ForMember(d => d.CanDeleteMessage, o => o.MapFrom(s => s.CanDeleteMessage));

            CreateMap<ChatMessage, ChatMessageDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ChatRoomId, o => o.MapFrom(s => s.ChatRoomId))
                .ForMember(d => d.SenderId, o => o.MapFrom(s => s.SenderId))
                .ForMember(d => d.SenderEmail, o => o.MapFrom(s => s.Sender != null ? (s.Sender.Email ?? string.Empty) : string.Empty))
                .ForMember(d => d.SenderName, o => o.MapFrom(s => s.Sender != null ? (s.Sender.FullName ?? string.Empty) : string.Empty))
                .ForMember(d => d.SenderProfilePicture, o => o.MapFrom(s => s.Sender != null ? (s.Sender.ProfilePicture ?? string.Empty) : string.Empty))
                .ForMember(d => d.Content, o => o.MapFrom(s => s.Content))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.Type))
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.IsEdited, o => o.MapFrom(s => s.IsEdited))
                .ForMember(d => d.EditedAt, o => o.MapFrom(s => s.EditedAt))
                .ForMember(d => d.IsDeleted, o => o.MapFrom(s => s.IsDeleted))
                .ForMember(d => d.CanEdit, o => o.Ignore())
                .ForMember(d => d.CanDelete, o => o.Ignore());

            CreateMap<ChatRoom, ChatRoomDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.Type))
                // CreatedByEmail/Name lấy từ navigation CreatedByUser
                .ForMember(d => d.CreatedByEmail, o => o.MapFrom(s => s.CreatedByUser != null ? (s.CreatedByUser.Email ?? string.Empty) : string.Empty))
                .ForMember(d => d.CreatedByName, o => o.MapFrom(s => s.CreatedByUser != null ? (s.CreatedByUser.FullName ?? string.Empty) : string.Empty))
                // Entity: CreatedAt -> DTO: CreatedDate
                .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.LastActivity, o => o.MapFrom(s => s.LastActivity))
                // Đếm participant đang active
                .ForMember(d => d.ParticipantCount, o => o.MapFrom(s => s.Participants != null ? s.Participants.Count(p => p.IsActive) : 0))
                // UnreadCount để service tính theo user hiện tại => ignore
                .ForMember(d => d.UnreadCount, o => o.Ignore())
                // LastMessage/LastMessageTime: lấy từ message mới nhất nếu đã Include
                .ForMember(d => d.LastMessage, o => o.Ignore())
                .ForMember(d => d.LastMessageTime, o => o.Ignore())
                .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive))
                // Map collection Participants -> ChatParticipantDto (cần Include User khi query)
                .ForMember(d => d.Participants, o => o.MapFrom(s => s.Participants));
            CreateMap<InquiryCreationDto, Inquiry>();
        }
    }
}
