using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rentify.BusinessObjects.DTO.ItemDto;
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
        private readonly IMapper _mapper;

        public CreateModel(IItemService itemService, ICategoryService categoryService, IMapper mapper)
        {
            _itemService = itemService;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [BindProperty]
        public Item Item { get; set; } = default!;

        public IEnumerable<SelectListItem> CategoryOptions { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var categories = await _categoryService.GetAllCategories();

            CategoryOptions = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
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

                return Page();
            }

            await _itemService.CreateItem(_mapper.Map<ItemCreateDto>(Item));

            return RedirectToPage("./Index");
        }
    }
}
