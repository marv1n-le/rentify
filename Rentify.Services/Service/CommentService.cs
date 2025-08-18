using Microsoft.AspNetCore.Http;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public CommentService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
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

        public async Task AddComment(Comment comment)
        {
            await _unitOfWork.CommentRepository.InsertAsync(comment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreateCommentAsync(string postId, string content)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User not authenticated");

            var comment = new Comment
            {
                UserId = userId,
                PostId = postId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CommentRepository.InsertAsync(comment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> CountCommentsByPostId(string postId)
        {
            return await _unitOfWork.CommentRepository.CountAsync(c => c.PostId == postId && !c.IsDeleted);
        }

        public async Task<bool> SoftDeleteComment(string commentId)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentId);
            if (comment == null) return false;

            var userId = GetCurrentUserId();
            var httpContext = _contextAccessor.HttpContext;
            var isAdmin = httpContext?.User?.IsInRole("Admin") ?? false;

            // Only allow Admin or comment owner
            if (!isAdmin && userId != comment.UserId)
                return false;

            comment.IsDeleted = true;
            await _unitOfWork.CommentRepository.UpdateAsync(comment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private string? GetCurrentUserId()
        {
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            // Try cookie first
            if (httpContext.Request?.Cookies.TryGetValue("userId", out var value) == true && !string.IsNullOrEmpty(value))
                return value;

            // Fallback to claims (works with auto-login)
            return httpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
    }
}