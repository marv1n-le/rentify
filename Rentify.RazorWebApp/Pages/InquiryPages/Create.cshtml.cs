using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.DTO.Inquiry;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;
using Rentify.Repositories.Helper;
using Rentify.Services.Interface;
using Rentify.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rentify.RazorWebApp.Pages.InquiryPages
{
    public class CreateModel : PageModel
    {
        private readonly IInquiryService _inquiryService;
        private readonly IPostService _postService;

        public CreateModel(IInquiryService inquiryService, IPostService postService)
        {
            _inquiryService = inquiryService;
            _postService = postService;
        }

        public IList<SelectListItem> PostOptions { get; set; }
        public IList<SelectListItem> StatusOptions { get; set; }

        [BindProperty]
        public InquiryCreationDto Inquiry { get; set; } = default!;

        private async Task LoadOptions()
        {
            var posts = await _postService.GetAllPost(new SearchFilterPostDto());

            PostOptions = posts.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Title
            }).ToList();

            StatusOptions = Enum.GetValues(typeof(InquiryStatus))
                                .Cast<InquiryStatus>()
                                .Select(s => new SelectListItem
                                {
                                    Value = ((int)s).ToString(),  
                                    Text = s.ToString()
                                }).ToList();
        }
        public async Task<IActionResult> OnGet()
        {
            await LoadOptions();
            return Page();
        }


        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadOptions();
            await _inquiryService.CreateInquiry(Inquiry);

            return RedirectToPage("./Index");
        }
    }
}
