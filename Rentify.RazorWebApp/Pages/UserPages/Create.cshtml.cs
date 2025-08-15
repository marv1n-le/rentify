using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rentify.BusinessObjects.Entities;

namespace Rentify.RazorWebApp.Pages.UserPages
{
    public class CreateModel : PageModel
    {
        private readonly Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext _context;

        public CreateModel(Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public User User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
