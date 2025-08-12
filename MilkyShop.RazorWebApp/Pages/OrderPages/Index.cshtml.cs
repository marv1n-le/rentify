using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MilkyShop.BusinessObjects.ApplicationDbContext;
using MilkyShop.BusinessObjects.Entities;

namespace MilkyShop.RazorWebApp.Pages.OrderPages
{
    public class IndexModel : PageModel
    {
        private readonly MilkyShop.BusinessObjects.ApplicationDbContext.MilkyShopDbContext _context;

        public IndexModel(MilkyShop.BusinessObjects.ApplicationDbContext.MilkyShopDbContext context)
        {
            _context = context;
        }

        public IList<Order> Order { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Order = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Discount)
                .Include(o => o.User).ToListAsync();
        }
    }
}
