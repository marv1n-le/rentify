using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Role
{
    public class IndexModel : PageModel
    {
        private readonly Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext _context;
        private readonly IRoleService _roleService;

        public IndexModel(Rentify.BusinessObjects.ApplicationDbContext.RentifyDbContext context, IRoleService roleService)
        {
            _context = context;
            _roleService = roleService;
        }

        public IList<Rental> Rental { get; set; } = default!;
        public IList<Rentify.BusinessObjects.Entities.Role> Roles { get; set; } = new List<Rentify.BusinessObjects.Entities.Role>();

        public async Task OnGetAsync()
        {
            Rental = await _context.Rentals
                .Include(r => r.User).ToListAsync();

            Roles = (await _roleService.GetAllRoles()).ToList();
        }
    }
}
