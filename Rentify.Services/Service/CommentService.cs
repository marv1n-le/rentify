using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Comment>> GetCommentByUserId(string userId)
        {
            return await _unitOfWork.CommentRepository.GetCommentByUserId(userId);
        }

        public async Task<List<Comment>> GetCommentByPostId(string postId)
        {
            return await _unitOfWork.CommentRepository.GetCommentByPostId(postId);
        }

        public async Task<List<Comment>> Get5NewestCommentByPostId(string postId)
        {
            return await _unitOfWork.CommentRepository.Get5NewestCommentByPostId(postId);
        }
    }
}