using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;
using Rentify.BusinessObjects.DTO.PostDto;

namespace Rentify.RazorWebApp.Pages.PostPages
{
    public class IndexModel : PageModel
    {
        private readonly IPostService _postService;

        public IndexModel(IPostService postService)
        {
            _postService = postService;
        }

        public IList<Post> Post { get; set; } = default!;

        public async Task OnGetAsync(int index = 1, int pageSize = 5)
        {
            Post = await _postService.GetAllPost(index, pageSize);
        }

        public async Task<IActionResult> OnGetMorePostsAsync(int index, int pageSize)
        {
            if (index < 1) index = 1;
            var posts = await _postService.GetAllPost(index, pageSize);
            return Partial("_PostCardList", posts);
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            try
            {
                var post = await _postService.GetPostById(id);
                if (post == null)
                {
                    return NotFound();
                }

                await _postService.DeletePost(id);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostCreateAsync([FromBody] CreatePostFormRequest request)
        {
            try
            {
                var postDto = new PostCreateRequestDto
                {
                    Title = request.Title,
                    Content = request.Content,
                    Tags = !string.IsNullOrEmpty(request.TagsString) 
                        ? request.TagsString.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList() 
                        : new List<string>(),
                    Images = !string.IsNullOrEmpty(request.ImagesString) 
                        ? request.ImagesString.Split(',').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)).ToList() 
                        : new List<string>()
                };

                var postId = await _postService.CreatePost(postDto);
                var createdPost = await _postService.GetPostById(postId);
                
                // Tạo response DTO đơn giản để tránh circular reference
                var responsePost = new PostResponseDto
                {
                    Id = createdPost.Id,
                    Title = createdPost.Title,
                    Content = createdPost.Content,
                    Tags = createdPost.Tags,
                    Images = createdPost.Images,
                    CreatedAt = createdPost.CreatedAt,
                    UserName = createdPost.User?.FullName,
                    UserProfilePicture = createdPost.User?.ProfilePicture
                };
                
                return new JsonResult(new { success = true, post = responsePost });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync([FromBody] UpdatePostFormRequest request)
        {
            try
            {
                var postDto = new PostUpdateRequestDto
                {
                    PostId = request.PostId,
                    Title = request.Title,
                    Content = request.Content,
                    Tags = !string.IsNullOrEmpty(request.TagsString) 
                        ? request.TagsString.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList() 
                        : new List<string>(),
                    Images = !string.IsNullOrEmpty(request.ImagesString) 
                        ? request.ImagesString.Split(',').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)).ToList() 
                        : new List<string>()
                };

                await _postService.UpdatePost(postDto);
                
                // Tạo response DTO với dữ liệu từ form update, không phải từ database
                var responsePost = new PostResponseDto
                {
                    Id = request.PostId,
                    Title = postDto.Title,
                    Content = postDto.Content,
                    Tags = postDto.Tags,
                    Images = postDto.Images,  // Sử dụng ảnh từ form update
                    CreatedAt = DateTime.Now, // Hoặc có thể lấy từ database nếu cần
                    UserName = "Current User", // Có thể lấy từ User context
                    UserProfilePicture = null
                };
                
                return new JsonResult(new { success = true, post = responsePost });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }

    // DTO tạm thời để nhận dữ liệu từ form
    public class CreatePostFormRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? TagsString { get; set; }
        public string? ImagesString { get; set; }
    }

    public class UpdatePostFormRequest
    {
        public string PostId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? TagsString { get; set; }
        public string? ImagesString { get; set; }
    }

    // DTO response đơn giản để tránh circular reference
    public class PostResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> Images { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; }
        public string? UserProfilePicture { get; set; }
    }
}
