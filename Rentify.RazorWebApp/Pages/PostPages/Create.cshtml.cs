using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.DTO.PostDto;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.PostPages
{
    public class CreateModel : PageModel
    {
        private readonly IPostService _postService;

        public CreateModel(IPostService postService)
        {
            _postService = postService;
        }

        [BindProperty]
        public PostCreateRequestDto Post { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            await _postService.CreatePost(Post);

            return RedirectToPage("./Index");
        }
    }
}
