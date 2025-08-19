using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.ItemPages
{
    public class DeleteModel : PageModel
    {
        private readonly IItemService _itemService;

        public DeleteModel(IItemService itemService)
        {
            _itemService = itemService;
        }

        [BindProperty]
        public Item Item { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _itemService.GetItemById(id);

            if (item == null)
            {
                return NotFound();
            }
            else
            {
                Item = item;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _itemService.DeleteItem(id);

            return RedirectToPage("./Index");
        }
    }
}
