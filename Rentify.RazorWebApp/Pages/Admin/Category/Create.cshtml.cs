using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin.Category
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateModel(ICategoryService categoryService, IUnitOfWork unitOfWork)
        {
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public BusinessObjects.Entities.Category Category { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _categoryService.CreateCategory(Category);

            return RedirectToPage("./Index");
        }
    }
}
