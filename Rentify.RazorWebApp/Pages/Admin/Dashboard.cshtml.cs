using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin;

[Authorize(Roles = "Admin")]
public class Dashboard : PageModel
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public Dashboard(IUserService userService,
        IRoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;
    }

    public int TotalUsers { get; set; }
    public int TotalItems { get; set; } = 0; // Tạm thời hardcode
    public int ActiveRentals { get; set; } = 0; // Tạm thời hardcode
    public int TotalRoles { get; set; } = 2; // Tạm thời hardcode (User, Admin)

    public async Task OnGetAsync()
    {
        var users = await _userService.GetAllUsers();
        TotalUsers = users.Count();

        // TODO: Thêm logic lấy thống kê cho Items, Rentals khi có service
        // TotalItems = await _itemService.GetTotalCount();
        // ActiveRentals = await _rentalService.GetActiveRentalsCount();
        // TotalRoles = await _roleService.GetTotalCount();
    }
}