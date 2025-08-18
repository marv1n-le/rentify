using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.DTO.UserDto;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class Create : PageModel
{
    private readonly IUserService _userService;

    public Create(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public SystemUserCreateDto SystemUser { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var result = await _userService.CreateSystemUser(SystemUser);

            if (result)
            {
                TempData["Message"] = "System user created successfully.";
                return RedirectToPage("/Admin/Users/Index");
            }
            else
            {
                TempData["Error"] = "Failed to create system user. Please try again.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"An error occurred: {ex.Message}";
            return Page();
        }
    }
}