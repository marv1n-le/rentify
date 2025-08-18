using Rentify.BusinessObjects.DTO.PostDto;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Helper;

namespace Rentify.Services.Interface
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPost(SearchFilterPostDto searchFilterPostDto);
        Task<Post> GetPostById(string postId);
        Task<string> CreatePost(PostCreateRequestDto request);
        Task UpdatePost(PostUpdateRequestDto request);
        Task DeletePost(string postId);
    }
}