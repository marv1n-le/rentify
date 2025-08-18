using Rentify.BusinessObjects.DTO.PostDto;
using Rentify.BusinessObjects.Entities;

namespace Rentify.Services.Interface
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPost(int index, int pageSize);
        Task<Post> GetPostById(string postId);
        Task<string> CreatePost(PostCreateRequestDto request);
        Task UpdatePost(PostUpdateRequestDto request);
        Task DeletePost(string postId);
    }
}