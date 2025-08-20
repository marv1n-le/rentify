using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.ItemPages
{
    public class IndexModel : PageModel
    {
        private readonly IItemService _itemService;

        public IndexModel(IItemService itemService)
        {
            _itemService = itemService;
        }

        public IList<Item> Item { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Item = (IList<Item>)await _itemService.GetAllItems();
        }
    }
}
