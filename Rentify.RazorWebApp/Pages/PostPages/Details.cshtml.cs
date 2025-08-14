using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.PostPages
{
    public class DetailsModel : PageModel
    {
        private readonly IPostService _postService;

        public DetailsModel(IPostService postService)
        {
            _postService = postService;
        }

        public Post Post { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _postService.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }
            else
            {
                Post = post;
            }
            return Page();
        }
    }
}
