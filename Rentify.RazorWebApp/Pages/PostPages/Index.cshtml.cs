using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rentify.BusinessObjects.DTO.PostDto;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;
using Rentify.Repositories.Helper;
using Rentify.Services.ExternalService.CloudinaryService;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.PostPages
{
    public class IndexModel : PageModel
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly IItemService _itemService;
        private readonly ICloudinaryService _cloudinaryService;

        public IndexModel(IPostService postService,
            ICommentService commentService,
            IUserService userService,
            IItemService itemService,
            ICloudinaryService cloudinaryService)
        {
            _postService = postService;
            _commentService = commentService;
            _userService = userService;
            _itemService = itemService;
            _cloudinaryService = cloudinaryService;
        }

        public class PostWithCommentCount
        {
            public Post Post { get; set; } = default!;
            public int CommentCount { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public SearchFilterPostDto SearchFilterPostDto { get; set; } = new();
        public List<SelectListItem> AvailableStatus { get; set; } = new();
        public IList<PostViewModel> Posts { get; set; } = default!;
        public IEnumerable<SelectListItem> ItemOptions { get; set; }

        [BindProperty]
        public List<IFormFile> PostImages { get; set; } = new();

        [BindProperty]
        public User CurrentUser { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (SearchFilterPostDto.PageIndex < 1)
                SearchFilterPostDto.PageIndex = 1;

            var posts = await _postService.GetAllPost(SearchFilterPostDto);
            var items = await _itemService.GetAllItemHasNoPost();

            ItemOptions = items.Select(i => new SelectListItem
            {
                Value = i.Id,
                Text = i.Name
            }).ToList();

            Posts = new List<PostViewModel>();
            CurrentUser = await _userService.GetUserById(_userService.GetCurrentUserId());

            foreach (var post in posts)
            {
                var count = await _commentService.CountCommentsByPostId(post.Id);
                Posts.Add(new PostViewModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Images = post.Images,
                    Tags = post.Tags,
                    CreatedAt = post.CreatedAt,
                    User = post.User,
                    CommentCount = count,
                    Status = (RentalStatus)post.Status,
                    Inquiries = post.Inquiries
                });
            }

            // Load AvailableStatus 
            AvailableStatus = Enum.GetValues(typeof(RentalStatus))
                .Cast<RentalStatus>()
                .Select(s => new SelectListItem
                {
                    Text = s.ToString(),
                    Value = s.ToString()
                })
                .ToList();
        }

        public async Task<IActionResult> OnGetMorePostsAsync(SearchFilterPostDto searchFilterPostDto)
        {
            if (searchFilterPostDto.PageIndex < 1)
                searchFilterPostDto.PageIndex = 1;

            var posts = await _postService.GetAllPost(searchFilterPostDto);

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

        public async Task<IActionResult> OnPostCreateAsync()
        {
            try
            {
                var imageList = new List<string>();

                // Process file uploads
                if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        if (file == null || file.Length == 0)
                            continue;
                        
                        if (file.Length > 5 * 1024 * 1024)
                        {
                            return new JsonResult(new
                            {
                                success = false,
                                message = $"File {file.FileName} is too large (max 5MB)"
                            });
                        }

                        // Validate file extensions
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(extension))
                        {
                            return new JsonResult(new
                            {
                                success = false,
                                message = $"File {file.FileName} has invalid extension"
                            });
                        }

                        try
                        {
                            var imageResult = await _cloudinaryService.AddPhotoAsync(file);
                            if (imageResult != null && !string.IsNullOrEmpty(imageResult.Url))
                            {
                                imageList.Add(imageResult.Url);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error uploading file {file.FileName}: {ex.Message}");
                        }
                    }
                }

                // Get form data
                var title = Request.Form["Title"].ToString();
                var content = Request.Form["Content"].ToString();
                var tagsString = Request.Form["Tags"].ToString();
                var itemId = Request.Form["ItemId"].ToString();
                var quantityString = Request.Form["Quantity"].ToString();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Title and content are required"
                    });
                }

                var postDto = new PostCreateRequestDto
                {
                    Title = title,
                    Content = content,
                    Tags = !string.IsNullOrEmpty(tagsString)
                        ? tagsString.Split(',')
                              .Select(t => t.Trim())
                              .Where(t => !string.IsNullOrEmpty(t))
                              .ToList()
                        : new List<string>(),
                    Images = imageList,
                    ItemId = itemId,
                    Quantity = int.TryParse(quantityString, out int qty) ? qty : 1
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
                    UserProfilePicture = createdPost.User?.ProfilePicture
                };

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnPostCreateAsync: {ex}");
                return new JsonResult(new
                {
                    success = false,
                    message = "An error occurred while creating the post",
                    error = ex.Message
                });
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            try
            {
                var imageList = new List<string>();

                // Process new image uploads
                if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                {
                    foreach (var file in Request.Form.Files)
                    {
                        if (file == null || file.Length == 0)
                            continue;

                        // Validate file size (5MB max)
                        if (file.Length > 5 * 1024 * 1024)
                        {
                            return new JsonResult(new
                            {
                                success = false,
                                message = $"File {file.FileName} is too large (max 5MB)"
                            });
                        }

                        // Validate file extensions
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(extension))
                        {
                            return new JsonResult(new
                            {
                                success = false,
                                message = $"File {file.FileName} has invalid extension"
                            });
                        }

                        try
                        {
                            var imageResult = await _cloudinaryService.AddPhotoAsync(file);
                            if (imageResult != null && !string.IsNullOrEmpty(imageResult.Url))
                            {
                                imageList.Add(imageResult.Url);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error uploading file {file.FileName}: {ex.Message}");
                        }
                    }
                }

                // Get form data
                var postId = Request.Form["PostId"].ToString();
                var title = Request.Form["Title"].ToString();
                var content = Request.Form["Content"].ToString();
                var tagsString = Request.Form["Tags"].ToString();
                var existingImagesString = Request.Form["ExistingImages"].ToString();

                // Combine old and new images
                var allImages = new List<string>();
                if (!string.IsNullOrEmpty(existingImagesString))
                {
                    allImages.AddRange(existingImagesString.Split(',').Select(url => url.Trim()).Where(url => !string.IsNullOrEmpty(url)));
                }
                allImages.AddRange(imageList);

                var postDto = new PostUpdateRequestDto
                {
                    PostId = postId,
                    Title = title,
                    Content = content,
                    Tags = !string.IsNullOrEmpty(tagsString)
                        ? tagsString.Split(',')
                              .Select(t => t.Trim())
                              .Where(t => !string.IsNullOrEmpty(t))
                              .ToList()
                        : new List<string>(),
                    Images = allImages,
                };

                await _postService.UpdatePost(postDto);

                // Get updated post data
                var updatedPost = await _postService.GetPostById(postId);
                var responsePost = new PostResponseDto
                {
                    Id = updatedPost.Id,
                    Title = updatedPost.Title,
                    Content = updatedPost.Content,
                    Tags = updatedPost.Tags,
                    Images = updatedPost.Images,
                    CreatedAt = updatedPost.CreatedAt,
                    UserName = updatedPost.User?.FullName,
                    UserProfilePicture = updatedPost.User?.ProfilePicture
                };

                return new JsonResult(new { success = true, post = responsePost });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnGetGetCommentsAsync(string postId)
        {
            try
            {
                var comments = await _commentService.GetCommentByPostId(postId) ?? new List<Comment>();
                return Partial("_CommentModalContent", Tuple.Create(postId, comments.AsEnumerable()));
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message + "<br/>" + ex.StackTrace, "text/html");
            }
        }

        public async Task<IActionResult> OnPostAddCommentAsync([FromBody] AddCommentRequest request)
        {
            try
            {
                await _commentService.CreateCommentAsync(request.PostId, request.Content);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public class DeleteCommentRequest
        {
            public string CommentId { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostDeleteCommentAsync([FromBody] DeleteCommentRequest request)
        {
            var result = await _commentService.SoftDeleteComment(request.CommentId);
            return new JsonResult(new { success = result });
        }
    }

    public class CreatePostFormRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? TagsString { get; set; }
        public string? ItemId { get; set; }
        public int? Quantity { get; set; }
        public List<string>? ImagesString { get; set; }
    }

    public class UpdatePostFormRequest
    {
        public string PostId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? TagsString { get; set; }
        public string? ExistingImages { get; set; }
    }

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

    public class AddCommentRequest
    {
        public string PostId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class PostViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<string> Images { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public User? User { get; set; }
        public int CommentCount { get; set; }
        public RentalStatus Status { get; set; }
        public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
        public int ItemQuantity { get; set; }
    }
}