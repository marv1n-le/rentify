using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;
using Rentify.BusinessObjects.DTO.PostDto; // Updated
using Rentify.BusinessObjects.DTO.CommentDto;

namespace Rentify.RazorWebApp.Pages.PostPages
{
    public class IndexModel : PageModel
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;

        public IndexModel(IPostService postService, ICommentService commentService)
        {
            _postService = postService;
            _commentService = commentService;
        }

        public IList<PostResponseDto> Posts { get; set; } = default!;

        public async Task OnGetAsync(int index = 1, int pageSize = 5)
        {
            var posts = await _postService.GetAllPost(index, pageSize);
            Posts = posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                Tags = p.Tags,
                Images = p.Images,
                CreatedAt = p.CreatedAt,
                UserName = p.User?.FullName,
                UserProfilePicture = p.User?.ProfilePicture,
                CommentCount = p.Comments?.Count ?? 0
            }).ToList();
        }

        public async Task<IActionResult> OnGetMorePostsAsync(int index, int pageSize)
        {
            var posts = await _postService.GetAllPost(index, pageSize);
            var dtoPosts = posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                Tags = p.Tags,
                Images = p.Images,
                CreatedAt = p.CreatedAt,
                UserName = p.User?.FullName,
                UserProfilePicture = p.User?.ProfilePicture,
                CommentCount = p.Comments?.Count ?? 0
            }).ToList();
            return Partial("_PostCardList", dtoPosts);
        }

        public async Task<IActionResult> OnGetCommentsAsync(string postId)
        {
            var comments = await _commentService.GetCommentByPostId(postId);
            var commentDtos = comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UserName = c.User?.FullName,
                UserProfilePicture = c.User?.ProfilePicture
            }).ToList();
            return Partial("_CommentsList", commentDtos);
        }

        public async Task<IActionResult> OnPostAddCommentAsync([FromBody] AddCommentRequest request)
        {
            try
            {
                var comment = new Comment
                {
                    PostId = request.PostId,
                    Content = request.Content,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                await _commentService.AddCommentAsync(comment);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
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

                var responsePost = new PostResponseDto
                {
                    Id = createdPost.Id,
                    Title = createdPost.Title,
                    Content = createdPost.Content,
                    Tags = createdPost.Tags,
                    Images = createdPost.Images,
                    CreatedAt = createdPost.CreatedAt,
                    UserName = createdPost.User?.FullName,
                    UserProfilePicture = createdPost.User?.ProfilePicture,
                    CommentCount = createdPost.Comments?.Count ?? 0
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

                var responsePost = new PostResponseDto
                {
                    Id = request.PostId,
                    Title = postDto.Title,
                    Content = postDto.Content,
                    Tags = postDto.Tags,
                    Images = postDto.Images,
                    CreatedAt = DateTime.Now,
                    UserName = "Current User",
                    UserProfilePicture = null,
                    CommentCount = 0 // You might want to fetch this from DB if needed
                };

                return new JsonResult(new { success = true, post = responsePost });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }

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

    public class AddCommentRequest
    {
        public string PostId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}