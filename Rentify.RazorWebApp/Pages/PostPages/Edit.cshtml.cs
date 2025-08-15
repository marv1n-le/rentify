using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.DTO.PostDto;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.PostPages
{
    public class EditModel : PageModel
    {
        private readonly IPostService _postService;

        public EditModel(IPostService postService)
        {
            _postService = postService;
        }

        [BindProperty]
        public PostUpdateRequestDto Post { get; set; } = default!;

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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _postService.UpdatePost(Post);

            return RedirectToPage("./Index");
        }
    }
}
