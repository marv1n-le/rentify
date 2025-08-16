using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Role
{
    public class DeleteModel : PageModel
    {
        private readonly IRoleService _roleService;

        public DeleteModel(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [BindProperty]
        public Rentify.BusinessObjects.Entities.Role Role { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleService.GetRoleById(id);
            if (role == null)
            {
                return NotFound();
            }
            Role = role;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleService.GetRoleById(id);
            if (role != null)
            {
                Role = role;
                Role.IsDeleted = true;
                await _roleService.UpdateRole(Role);
            }

            return RedirectToPage("./Index");
        }
    }
}
