using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rentify.BusinessObjects.DTO.RentalDTO;
using Rentify.BusinessObjects.Enum;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Rentals
{
    public class CreateModel : PageModel
    {
        private readonly IRentalService _rentalService;

        public CreateModel(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }
        [BindProperty]
        public RentalCreateDTO Rental { get; set; } = default!;
        public IEnumerable<SelectListItem> StatusList { get; set; } = [];
        public IEnumerable<SelectListItem> PaymentStatusList { get; set; } = [];
        public void OnGet()
        {
            StatusList = Enum.GetValues(typeof(RentalStatus))
                .Cast<RentalStatus>()
                .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });

            PaymentStatusList = Enum.GetValues(typeof(PaymentStatus))
                .Cast<PaymentStatus>()
                .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }
            await _rentalService.CreateRental(Rental);
            return RedirectToPage("./Index");
        }
    }
}
