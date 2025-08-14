using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.DTO.PostDto;
using Rentify.BusinessObjects.Entities;
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
