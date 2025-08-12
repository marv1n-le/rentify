using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MilkyShop.BusinessObjects.ApplicationDbContext;
using MilkyShop.BusinessObjects.Entities;

namespace MilkyShop.RazorWebApp.Pages.FeedbackPages
{
    public class DetailsModel : PageModel
    {
        private readonly MilkyShop.BusinessObjects.ApplicationDbContext.MilkyShopDbContext _context;

        public DetailsModel(MilkyShop.BusinessObjects.ApplicationDbContext.MilkyShopDbContext context)
        {
            _context = context;
        }

        public Feedback Feedback { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }
            else
            {
                Feedback = feedback;
            }
            return Page();
        }
    }
}
