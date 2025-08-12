using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MilkyShop.Repositories.Implement;
using MilkyShop.Services.Interface;
using MilkyShop.Services.Service;

namespace MilkyShop.RazorWebApp.Pages.Account;

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
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;


    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        var account = await _service.GetUserAccount(Username, Password);

        if (account != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, Username),
                new Claim(ClaimTypes.Role, account.RoleId.ToString()),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            Response.Cookies.Append("UserName", account.Username);
            return RedirectToPage("/Index");
        }
        else
        {
            TempData["Message"] = "Login fail, please check your account";
        }

        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Page();
    }
}