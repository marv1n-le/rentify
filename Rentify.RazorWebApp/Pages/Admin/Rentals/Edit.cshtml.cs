using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.DTO.RentalDTO;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;
using Rentify.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rentify.RazorWebApp.Pages.Rentals
{
    public class EditModel : PageModel
    {
        private readonly IRentalService _rentalService;
        public EditModel(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [BindProperty]
        public RentalUpdateDTO Rental { get; set; } = default!;
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

            // Map the entity to the DTO and set RentalId
            Rental = new RentalUpdateDTO
            {
                RentalId = rental.Id,
                RentalDate = rental.RentalDate,
                ReturnDate = rental.ReturnDate,
                TotalAmount = rental.TotalAmount,
                Status = rental.Status,
                PaymentStatus = rental.PaymentStatus
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _rentalService.UpdateRental(Rental);

            return RedirectToPage("./Index");
        }
    }
}
