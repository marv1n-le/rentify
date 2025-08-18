using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin.Category
{
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;

        public EditModel(ICategoryService categoryService, IUnitOfWork unitOfWork)
        {
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Rentify.BusinessObjects.Entities.Category Category { get; set; } = default!;

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

            Category = category;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var category = await _categoryService.GetCategoryById(Category.Id);

            if(category == null)
            {
                return NotFound();
            }

            //TODO: Need to add validation

            category.Name = Category.Name ?? "";
            category.Description = Category.Description;

            await _categoryService.UpdateCategory(category);
            return RedirectToPage("./Index");
        }
    }
}
