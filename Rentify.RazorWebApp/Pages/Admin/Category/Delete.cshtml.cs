using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin.Category
{
    public class DeleteModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteModel(ICategoryService categoryService, IUnitOfWork unitOfWork)
        {
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public BusinessObjects.Entities.Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryById(id);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                Category = category;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _categoryService.GetCategoryById(id);
            if (category != null)
            {
                Category = category;
                Category.IsDeleted = true;
                await _categoryService.UpdateCategory(Category);
            }

            return RedirectToPage("./Index");
        }
    }
}
