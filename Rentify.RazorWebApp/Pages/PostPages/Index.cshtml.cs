using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

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
    }
}
