using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.ApplicationDbContext;
using Rentify.BusinessObjects.Entities;

namespace Rentify.RazorWebApp.Pages.InquiryPages
{
    public class EditModel : PageModel
    {
        private readonly Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext _context;

        public EditModel(Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Inquiry Inquiry { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry =  await _context.Inquiries.FirstOrDefaultAsync(m => m.Id == id);
            if (inquiry == null)
            {
                return NotFound();
            }
            Inquiry = inquiry;
           ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id");
           ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Inquiry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InquiryExists(Inquiry.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool InquiryExists(string id)
        {
            return _context.Inquiries.Any(e => e.Id == id);
        }
    }
}
