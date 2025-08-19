using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.DTO.RentalDTO;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin.Rentals
{
    public class IndexModel : PageModel
    {
        private readonly IRentalService _rentalService;
        private readonly IInquiryService _inquiryService;

        public IndexModel(IRentalService rentalService, IInquiryService inquiryService)
        {
            _rentalService = rentalService;
            _inquiryService = inquiryService;
        }

        public List<Rental> Rental { get; set; } = new List<Rental>();
        public List<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            Rental = await _rentalService.GetAllRental();
            Inquiries = (await _inquiryService.GetAllInquiries()).ToList();
        }

        public async Task<IActionResult> OnPostConfirmAsync(string id)
        {
            await _rentalService.ConfirmRental(id);
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostActivateAsync(string id)
        {
            await _rentalService.ActivateRental(id);
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostCompleteAsync(string id)
        {
            await _rentalService.CompleteRental(id, 0, 0);
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostCancelAsync(string id)
        {
            await _rentalService.CancelRental(id);
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostApproveInquiryAsync(string inquiryId)
        {
            var rentalDto = new RentalCreateDTO();
            try
            {
                await _rentalService.CreateFromInquiryAsync(inquiryId, rentalDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
            }

            return RedirectToPage("./Index");
        }
    }
}