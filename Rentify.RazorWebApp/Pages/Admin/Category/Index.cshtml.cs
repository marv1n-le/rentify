using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin.Category
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;
        public IList<BusinessObjects.Entities.Category> Category { get; set; } = default!;

        public IndexModel(ICategoryService categoryService, IUnitOfWork unitOfWork)
        {
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
        }

        public async Task OnGetAsync()
        {
            Category = (IList<BusinessObjects.Entities.Category>)await _categoryService.GetAllCategories();
        }
    }
}
