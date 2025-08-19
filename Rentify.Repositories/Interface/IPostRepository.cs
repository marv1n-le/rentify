using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Helper;
using Rentify.Repositories.Implement;

namespace Rentify.Repositories.Interface
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<List<Post>> GetAllPost(SearchFilterPostDto searchFilterPostDto);
        Task<List<Post>> GetAllPost();
        Task<Post> GetById(string postId);
    }
}
