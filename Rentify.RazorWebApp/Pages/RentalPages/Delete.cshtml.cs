using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;

namespace Rentify.RazorWebApp.Pages.RentalPages
{
    public class DeleteModel : PageModel
    {
        private readonly Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext _context;

        public DeleteModel(Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Rental Rental { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FirstOrDefaultAsync(m => m.Id == id);

            if (rental == null)
            {
                return NotFound();
            }
            else
            {
                Rental = rental;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FindAsync(id);
            if (rental != null)
            {
                Rental = rental;
                _context.Rentals.Remove(Rental);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
