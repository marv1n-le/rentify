using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;

namespace Rentify.RazorWebApp.Pages.Rentals
{
    public class IndexModel : PageModel
    {
        private readonly Rentify.BusinessObjects.ApplicationDbContext.MilkyShopDbContext _context;

        public IndexModel(Rentify.BusinessObjects.ApplicationDbContext.MilkyShopDbContext context)
        {
            _context = context;
        }

        public IList<Rental> Rental { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Rental = await _context.Rentals
                .Include(r => r.User).ToListAsync();
        }
    }
}
