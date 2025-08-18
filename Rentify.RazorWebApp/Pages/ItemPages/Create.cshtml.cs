using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rentify.RazorWebApp.Pages.ItemPages
{
    public class CreateModel : PageModel
    {
        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateModel(IItemService itemService, ICategoryService categoryService, IUnitOfWork unitOfWork)
        {
            _itemService = itemService;
            _categoryService = categoryService;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Item Item { get; set; } = default!;

        public IEnumerable<SelectListItem> CategoryOptions { get; set; }
        public IEnumerable<SelectListItem> StatusOptions { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var categories = await _categoryService.GetAllCategories();

            CategoryOptions = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            StatusOptions = Enum.GetValues(typeof(ItemStatus))
                .Cast<ItemStatus>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = s.ToString()
                }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllCategories();
                CategoryOptions = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                StatusOptions = Enum.GetValues(typeof(ItemStatus))
                    .Cast<ItemStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = ((int)s).ToString(),
                        Text = s.ToString()
                    }).ToList();

                return Page();
            }

            await _itemService.CreateItem(Item);

            return RedirectToPage("./Index");
        }
    }
}
