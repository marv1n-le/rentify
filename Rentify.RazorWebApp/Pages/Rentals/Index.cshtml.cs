using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Rentals
{
    public class IndexModel : PageModel
    {
        private readonly IRentalService _rentalService;
        public IndexModel(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }
        public IList<Rental> Rental { get; set; } = default!;
        public async Task OnGetAsync()
        {
            Rental = await _rentalService.GetAllRental();
        }
    }
}
