using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.DTO.UserDto;
using Rentify.Services.Interface;

namespace Rentify.RazorWebApp.Pages.Account;

[Authorize]
public class Profile : PageModel
{
    private readonly IUserService _userService;

    public Profile(IUserService userService)
    {
        _userService = userService;
    }

    public UserResponseDto Data { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {
        var userId = _userService.GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
            return RedirectToPage("/Account/Login"); // Khi đã fix claim, sẽ không vào nhánh này nữa

        var user = await _userService.GetUserById(userId);
        if (user == null) return RedirectToPage("/Account/Login");

        Data = new UserResponseDto
        {
            FullName = user.FullName,
            ProfilePicture = user.ProfilePicture,
            BirthDate = user.BirthDate,
            RoleName = user.Role?.Name,
            Email = user.Email
        };

        return Page();
    }
}