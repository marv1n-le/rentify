using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Role
{
    public class CreateModel : PageModel
    {
        private readonly IRoleService _roleService;

        public CreateModel(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [BindProperty]
        public Rentify.BusinessObjects.Entities.Role Role { get; set; } = new Rentify.BusinessObjects.Entities.Role();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _roleService.CreateRole(Role);
            return RedirectToPage("./Index");
        }
    }
}
