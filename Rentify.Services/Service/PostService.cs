using AutoMapper;
using Microsoft.AspNetCore.Http;
using Rentify.BusinessObjects.DTO.PostDto;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Helper;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.Services.Service
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public PostService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        public async Task<string> CreatePost(PostCreateRequestDto post)
        {
            var userId = GetCurrentUserId();
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception($"Please log in first");

            var item = await _unitOfWork.ItemRepository.GetByIdAsync(post.ItemId);
            if (item == null)
                throw new Exception($"Item with id: {post.ItemId} not found");

            Post newPost = new Post
            {
                UserId = userId,
                User = user,
                Content = post.Content,
                Images = post.Images,
                Tags = post.Tags,
                Title = post.Title,
                Item = item,
                ItemId = item.Id,
            };

            await _unitOfWork.PostRepository.InsertAsync(newPost);
            var rowsAffected = await _unitOfWork.SaveChangesAsync();
            var savedPost = await _unitOfWork.PostRepository.GetById(newPost.Id);
            if (rowsAffected <= 0)
            {
                throw new Exception("Không thể lưu bài đăng vào database");
            }
            if (savedPost == null)
            {
                throw new Exception("Failed to save post in the database.");
            }
            return newPost.Id;
        }

        public async Task DeletePost(string postId)
        {
            var post = await _unitOfWork.PostRepository.GetById(postId);

            if (post == null)
                throw new Exception($"Post with id: {postId} has not found");

            await _unitOfWork.PostRepository.SoftDeleteAsync(post);
        }

        public async Task<List<Post>> GetAllPost(SearchFilterPostDto searchFilterPostDto)
        {
            var postList = await _unitOfWork.PostRepository.GetAllPost(searchFilterPostDto);

            if (postList == null)
                throw new Exception("Has no record for Post");

            return postList;
        }

        public async Task<Post> GetPostById(string postId)
        {
            var post = await _unitOfWork.PostRepository.GetById(postId);

            if (post == null)
                throw new Exception($"Post with id: {postId} has not found");

            return post;
        }

        public async Task UpdatePost(PostUpdateRequestDto request)
        {
            var post = await _unitOfWork.PostRepository.GetById(request.PostId);

            if (post == null)
                throw new Exception($"Post with id: {request.PostId} has not found");

            _mapper.Map(request, post);

            await _unitOfWork.PostRepository.UpdateAsync(post);
            await _unitOfWork.SaveChangesAsync();
        }

        private string GetCurrentUserId()
        {
            var userId = _contextAccessor.HttpContext.Request.Cookies.TryGetValue("userId", out var value) ? value.ToString() : null;
            return userId;
        }
    }
}
