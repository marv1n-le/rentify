using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;

namespace Rentify.Repositories.Interface
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<List<Comment>> GetCommentByUserId(string userId);
        Task<List<Comment>> GetCommentByPostId(string postId);
        Task<List<Comment>> Get5NewestCommentByPostId(string postId);
    }
}