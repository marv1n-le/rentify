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
                new Claim(ClaimTypes.Role, account.Role!.Name!),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            Response.Cookies.Append("UserName", account.Username!);
            Response.Cookies.Append("userId", account.Id);
            
            if ( account.Role != null && account.Role.Name == "Admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
        else
        {
            TempData["Message"] = "Login fail, please check your account";
        }

        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Page();
    }
}