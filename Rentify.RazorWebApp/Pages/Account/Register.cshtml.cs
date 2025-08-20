using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.BusinessObjects.DTO.UserDto;
using Rentify.Services.Interface;
using System.ComponentModel.DataAnnotations;
using Rentify.Services.ExternalService.CloudinaryService;

namespace Rentify.RazorWebApp.Pages.Account;

[AllowAnonymous]
public class Register : PageModel
{
    private readonly IUserService _userService;
    private readonly ICloudinaryService _cloudinary;
    public Register(IUserService userService,
        ICloudinaryService cloudinary)
    {
        _cloudinary = cloudinary;
        _userService = userService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

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

    [BindProperty] public DateTime? BirthDate { get; set; }

    [BindProperty]
    [Display(Name = "Ảnh đại diện")]
    public IFormFile? ProfileImage { get; set; }

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
            string? profileUrl = null;

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
                if (!allowed.Contains(ProfileImage.ContentType))
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng chọn ảnh .jpg/.png/.webp");
                    return Page();
                }

                if (ProfileImage.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(string.Empty, "Ảnh tối đa 5MB.");
                    return Page();
                }

                var upload = await _cloudinary.AddPhotoAsync(ProfileImage);
                if (!upload.IsSuccess || string.IsNullOrEmpty(upload.Url))
                {
                    ModelState.AddModelError(string.Empty, $"Upload ảnh thất bại: {upload.ErrorMessage}");
                    return Page();
                }

                profileUrl = upload.Url;
            }

            if (BirthDate.HasValue)
                BirthDate = DateTime.SpecifyKind(BirthDate.Value, DateTimeKind.Utc);
            
            var registerDto = new UserRegisterDto
            {
                Email = Email,
                Password = Password,
                FullName = FullName,
                BirthDate = BirthDate,
                ProfilePicture = profileUrl
            };

            await _userService.CreateUser(registerDto);

            TempData["Message"] = "Đăng ký thành công! Vui lòng nhập OTP được gửi tới email.";
            return RedirectToPage("/Account/Verify", new { email = Email });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return Page();
        }
    }
}