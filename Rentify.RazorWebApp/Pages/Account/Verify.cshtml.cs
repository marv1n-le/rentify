using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rentify.Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace Rentify.RazorWebApp.Pages.Account
{
    [AllowAnonymous]
    public class Verify : PageModel
    {
        private readonly IOtpService _otp;
        public Verify(IOtpService otp) { _otp = otp; }

        [BindProperty(SupportsGet = true)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<IActionResult> OnPostVerify()
        {
            try
            {
                var ok = await _otp.VerifyAsync(Email, Code);
                if (ok)
                {
                    TempData["Message"] = "Xác thực thành công! Bây giờ bạn có thể đăng nhập.";
                    return RedirectToPage("/Account/Login");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostResend()
        {
            try
            {
                await _otp.ResendAsync(Email);
                TempData["Message"] = "Đã gửi lại OTP. Vui lòng kiểm tra email.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToPage(new { email = Email });
        }
    }
}