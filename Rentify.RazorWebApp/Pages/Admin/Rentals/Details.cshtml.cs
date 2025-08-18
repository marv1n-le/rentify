using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin.Rentals
{
    public class DetailsModel : PageModel
    {
        private readonly IRentalService _rentalService;

        public DetailsModel(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }
        public Rental Rental { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _rentalService.GetRentalById(id);
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
    }
}
