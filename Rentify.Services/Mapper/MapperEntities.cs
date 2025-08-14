using AutoMapper;
using Rentify.BusinessObjects.DTO.PostDto;
using Rentify.BusinessObjects.Entities;

namespace Rentify.Services.Mapper
{
    public class MapperEntities : Profile
    {
        public MapperEntities()
        {
            CreateMap<Post, PostUpdateRequestDto>().ReverseMap();
        }
    }
}
