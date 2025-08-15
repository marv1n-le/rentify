using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.DTO.UserDto;
using Rentify.Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace Rentify.RazorWebApp.Pages.Account;

[AllowAnonymous]
public class Register : PageModel
{
    private readonly IUserService _userService;

    public Register(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Họ và tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    public DateTime? BirthDate { get; set; }


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
            var registerDto = new UserRegisterDto
            {
                Username = Username,
                Password = Password,
                FullName = FullName,
                BirthDate = BirthDate,
            };

            await _userService.CreateUser(registerDto);

            TempData["Message"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToPage("/Account/Login");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return Page();
        }
    }
}
