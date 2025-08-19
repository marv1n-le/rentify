using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.ItemPages
{
    public class DetailsModel : PageModel
    {
        private readonly IItemService _itemService;

        public DetailsModel(IItemService itemService)
        {
            _itemService = itemService;
        }

        public Item Item { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Item = await _itemService.GetItemById(id);

            return Page();
        }
    }
}
