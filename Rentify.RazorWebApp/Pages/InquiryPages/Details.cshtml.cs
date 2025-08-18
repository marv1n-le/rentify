using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;

namespace Rentify.RazorWebApp.Pages.InquiryPages
{
    public class DetailsModel : PageModel
    {
        private readonly Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext _context;

        public DetailsModel(Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext context)
        {
            _context = context;
        }

        public Inquiry Inquiry { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiries.FirstOrDefaultAsync(m => m.Id == id);
            if (inquiry == null)
            {
                return NotFound();
            }
            else
            {
                Inquiry = inquiry;
            }
            return Page();
        }
    }
}
