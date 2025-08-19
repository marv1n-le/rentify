using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.Enum;
using Rentify.RazorWebApp.Pages.PostPages;
using Rentify.Repositories.Helper;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly ICommentService _commentService;

        public IndexModel(IPostService postService, IMapper mapper, ICommentService commentService)
        {
            _postService = postService;
            _mapper = mapper;
            _commentService = commentService;
        }

        [BindProperty]
        public List<PostViewModel> Posts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public SearchFilterPostDto SearchFilterPostDto { get; set; } = new();

        public async Task OnGet()
        {
            var postList = await _postService.GetAllPost(SearchFilterPostDto);

            Posts = new List<PostViewModel>();

            foreach (var post in postList)
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
        }
    }
}
