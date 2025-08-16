using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Role
{
    public class DetailsModel : PageModel
    {
        private readonly IRoleService _roleService;

        public DetailsModel(IRoleService roleService)
        {
            _roleService = roleService;
        }

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
    }
}
