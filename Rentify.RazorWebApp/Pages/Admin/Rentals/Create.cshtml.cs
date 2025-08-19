using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rentify.BusinessObjects.DTO.RentalDTO;
using Rentify.BusinessObjects.Enum;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin.Rentals
{
    public class CreateModel : PageModel
    {
        private readonly IRentalService _rentalService;

        public CreateModel(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [BindProperty]
        public RentalCreateDTO Rental { get; set; } = new RentalCreateDTO { RentalItems = new List<RentalItemDTO>() };

        public IEnumerable<SelectListItem> StatusList { get; set; } = [];
        public IEnumerable<SelectListItem> PaymentStatusList { get; set; } = [];

        public void OnGet()
        {
            Rental.RentalItems.Add(new RentalItemDTO());
            StatusList = Enum.GetValues(typeof(RentalStatus))
                .Cast<RentalStatus>()
                .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });

            PaymentStatusList = Enum.GetValues(typeof(PaymentStatus))
                .Cast<PaymentStatus>()
                .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || !Rental.RentalItems.Any())
            {
                ModelState.AddModelError("", "At least one RentalItem is required");
                OnGet();
                return Page();
            }

            await _rentalService.CreateRental(Rental);
            return RedirectToPage("./Index");
        }
    }
}