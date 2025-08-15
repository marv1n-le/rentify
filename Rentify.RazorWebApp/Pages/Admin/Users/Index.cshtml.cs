using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.Entities;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class Index : PageModel
{
    private readonly IUserService _userService;

    public Index(IUserService userService)
    {
        _userService = userService;
    }

    public IEnumerable<User>? Users { get; set; }

    public async Task OnGetAsync()
    {
        Users = await _userService.GetAllUsers();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string userId)
    {
        try
        {
            var result = await _userService.SoftDeleteUser(userId);
            if (result)
            {
                TempData["Message"] = "User deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete user.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting user: {ex.Message}";
        }

        return RedirectToPage();
    }
}