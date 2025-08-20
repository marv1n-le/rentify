using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Repositories.Implement;
using Rentify.Services.Interface;
using System.Security.Claims;

namespace Rentify.RazorWebApp.Pages.Account;

[AllowAnonymous]
public class Login : PageModel
{
    private readonly IUserService _service;
    private readonly IUnitOfWork _unitOfWork;

    public Login(IUserService service, IUnitOfWork unitOfWork)
    {
        _service = service;
        _unitOfWork = unitOfWork;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;


    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        var account = await _service.GetUserAccount(Email, Password);

        if (account != null)
        {
            var roleName = account.Role?.Name ?? "User";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id),
                new Claim(ClaimTypes.Name, account.Email ?? string.Empty),
                new Claim(ClaimTypes.Email, account.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, roleName),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            
            Response.Cookies.Append("UserEmail", account.Email!);
            Response.Cookies.Append("UserName", account.FullName!);
            Response.Cookies.Append("userId", account.Id);

            if (roleName == "Admin")
                return RedirectToPage("/Admin/Dashboard");

            if (roleName == "User")
                return RedirectToPage("/PostPages/Index");
            return RedirectToPage("/Index");
        }

        TempData["Message"] = "Login fail, please check your account";
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Page();
    }

}