using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;

namespace Rentify.Repositories.Interface
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<List<Post>> GetAllPost(int index, int pageSize);
        Task<Post> GetById(string postId);
    }
}
