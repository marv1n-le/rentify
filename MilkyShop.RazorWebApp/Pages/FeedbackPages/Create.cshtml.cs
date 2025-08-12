using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MilkyShop.BusinessObjects.ApplicationDbContext;
using MilkyShop.BusinessObjects.Entities;

namespace MilkyShop.RazorWebApp.Pages.FeedbackPages
{
    public class CreateModel : PageModel
    {
        private readonly MilkyShop.BusinessObjects.ApplicationDbContext.MilkyShopDbContext _context;

        public CreateModel(MilkyShop.BusinessObjects.ApplicationDbContext.MilkyShopDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["OrderItemId"] = new SelectList(_context.OrderItems, "Id", "Id");
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Feedback Feedback { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Feedbacks.Add(Feedback);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
