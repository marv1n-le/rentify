using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.DTO.ItemDto;
using Rentify.BusinessObjects.Entities;
using Rentify.BusinessObjects.Enum;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.ItemPages
{
    public class EditModel : PageModel
    {
        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public EditModel(IItemService itemService, ICategoryService categoryService, IMapper mapper)
        {
            _itemService = itemService;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [BindProperty]
        public Item Item { get; set; } = default!;

        public IEnumerable<SelectListItem> StatusOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {

            Item = await _itemService.GetItemById(id);

            var listCategory = await _categoryService.GetAllCategories();

            ViewData["CategoryId"] = new SelectList(listCategory, "Id", "Name");

            StatusOptions = Enum.GetValues(typeof(ItemStatus))
                .Cast<ItemStatus>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = s.ToString()
                }).ToList();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            await _itemService.UpdateItem(_mapper.Map<ItemUpdateDto>(Item));

            return RedirectToPage("./Index");
        }
    }
}
