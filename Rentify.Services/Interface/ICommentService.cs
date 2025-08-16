using Rentify.BusinessObjects.Entities;

namespace Rentify.Services.Interface
{
    public interface ICommentService
    {
        Task<List<Comment>> GetCommentByUserId(string userId);
        Task<List<Comment>> GetCommentByPostId(string postId);
        Task<List<Comment>> Get5NewestCommentByPostId(string postId);
        Task AddCommentAsync(Comment comment);
    }
}