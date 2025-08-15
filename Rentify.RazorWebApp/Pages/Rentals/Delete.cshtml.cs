using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;
using Rentify.Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rentify.RazorWebApp.Pages.Rentals
{
    public class DeleteModel : PageModel
    {
        private readonly IRentalService _rentalService;

        public DeleteModel(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }
        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _rentalService.DeleteRental(id);

            return RedirectToPage("./Index");
        }
    }
}
