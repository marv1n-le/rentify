using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;

namespace Rentify.RazorWebApp.Pages.RentalPages
{
    public class IndexModel : PageModel
    {
        private readonly Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext _context;

        public IndexModel(Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext context)
        {
            _context = context;
        }

        public IList<Rental> Rental { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Rental = await _context.Rentals
                .Include(r => r.User).ToListAsync();
        }
    }
}
