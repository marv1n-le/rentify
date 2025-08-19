using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin.Rentals
{
    public class IndexModel : PageModel
    {
        private readonly IRentalService _rentalService;
        public IndexModel(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        public List<Rental> Rental { get; set; }

        public async Task OnGetAsync()
        {
            Rental = await _rentalService.GetAllRental();
            
        }

        public async Task<IActionResult> OnPostConfirmAsync(string id)
        {
            await _rentalService.ConfirmRental(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostActivateAsync(string id)
        {
            await _rentalService.ActivateRental(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCompleteAsync(string id)
        {
            await _rentalService.CompleteRental(id, 0, 0); 
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelAsync(string id)
        {
            await _rentalService.CancelRental(id);
            return RedirectToPage();
        }
    }
}
