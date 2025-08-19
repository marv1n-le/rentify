using System.Security.Cryptography;
using Rentify.BusinessObjects.Entities;
using Rentify.Repositories.Implement;
using Rentify.Services.ExternalService.MailGun;
using Rentify.Services.Interface;

namespace Rentify.Services.Service
{
    public class OtpService : IOtpService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailSenderService _email;

        public OtpService(IUnitOfWork uow, IEmailSenderService email)
        {
            _uow = uow;
            _email = email;
        }

        private static string NewCode()
        {
            var num = RandomNumberGenerator.GetInt32(0, 1_000_000);
            return num.ToString("D6");
        }

        public async Task GenerateAndSendAsync(string userId)
        {
            var user = await _uow.UserRepository.GetUserById(userId)
                       ?? throw new Exception("User not found");

            // Xoá OTP cũ
            var oldOtps = await _uow.OtpRepository.FindAllAsync(x => x.UserId == userId);
            if (oldOtps.Any())
                _uow.OtpRepository.DeleteRange(oldOtps);

            // Tạo OTP mới
            var code = NewCode(); // 6 chữ số, ví dụ: 123456
            var otp = new Otp
            {
                UserId = userId,
                Code = code, // (Bonus bảo mật: lưu dạng hash)
                MetaData = "register",
                ExpiredAt = DateTime.UtcNow.AddMinutes(5)
            };

            await _uow.OtpRepository.InsertAsync(otp);
            await _uow.SaveChangesAsync();

            var subject = "Rentify – Mã xác thực đăng ký (OTP)";

            // Tuỳ route của bạn – nếu có trang nhập OTP riêng, đính kèm link để “tech” hơn
            var verifyUrl =
                $"https://rentify.marv-levolution.cloud/Account/Verify?email={Uri.EscapeDataString(user.Email!)}";

            var html = BuildOtpEmail(user.FullName, user.Email!, code, TimeSpan.FromMinutes(5), verifyUrl);

            await _email.SendEmailAsync(user.Email!, subject, html);
        }

        public async Task<bool> VerifyAsync(string email, string code)
        {
            var user = await _uow.UserRepository.FindAsync(u => u.Email == email)
                       ?? throw new Exception("User not found");

            var now = DateTime.UtcNow;
            var otp = (await _uow.OtpRepository
                    .FindAllAsync(x => x.UserId == user.Id && x.ExpiredAt > now))
                .OrderByDescending(x => x.ExpiredAt)
                .FirstOrDefault();

            if (otp == null)
                throw new Exception("OTP đã hết hạn hoặc không tồn tại.");

            if (!string.Equals(otp.Code, code, StringComparison.Ordinal))
                throw new Exception("OTP không đúng.");

            user.IsVerify = true;
            _uow.UserRepository.UpdateAsync(user);

            var all = await _uow.OtpRepository.FindAllAsync(x => x.UserId == user.Id);
            if (all.Any()) _uow.OtpRepository.DeleteRange(all);

            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task ResendAsync(string email)
        {
            var user = await _uow.UserRepository.FindAsync(u => u.Email == email)
                       ?? throw new Exception("User not found");

            await GenerateAndSendAsync(user.Id);
        }

        private static string BuildOtpEmail(string? fullNameOrNull, string email, string code, TimeSpan ttl,
            string? verifyUrl = null)
        {
            var name = string.IsNullOrWhiteSpace(fullNameOrNull) ? email : fullNameOrNull;
            var minutes = (int)Math.Ceiling(ttl.TotalMinutes);
            var preheader = $"Mã OTP của bạn là {code}. Hiệu lực trong {minutes} phút.";

            string displayCode = code?.Trim() ?? "";
            // Mã OTP không còn khoảng trắng, giờ là một chuỗi liền nhau
            displayCode = displayCode.Length == 6 ? displayCode : "XXXXXX"; // fallback nếu không phải mã 6 số

            var brand = new
            {
                Name = "Rentify",
                Logo =
                    "https://res.cloudinary.com/di9xfkskd/image/upload/v1755622123/rentify_photos/rentify_fc9f4bc7-59e9-4f21-9114-102ba82b1818.png",
                Primary = "#7C3AED", // Màu tím
                Bg = "#F5F7FA", // Màu nền sáng
                Card = "#FFFFFF", // Màu thẻ trắng
                Text = "#333333", // Màu chữ chính
                Muted = "#7F7F7F", // Màu chữ phụ
                ButtonBg = "#7C3AED", // Nút chính màu tím
                ButtonText = "#FFFFFF" // Nút chính có chữ trắng
            };

            var html = $@"
<!doctype html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <title>{brand.Name} – OTP</title>
  <style>
    /* Preheader ẩn */
    .preheader {{ display:none!important; visibility:hidden; opacity:0; color:transparent; height:0; width:0; overflow:hidden; }}
    /* Dark mode (được Gmail/WebKit hỗ trợ) */
    @media (prefers-color-scheme: dark) {{
      :root {{
        color-scheme: dark;
      }}
    }}
    @media only screen and (max-width: 600px) {{
      .container {{ width: 100% !important; }}
      .card {{ padding: 24px !important; }}
      .otp {{ font-size: 28px !important; }}
      .btn {{ display:block !important; width:100% !important; }}
    }}
  </style>
</head>
<body style=""margin:0; padding:0; background:{brand.Bg};"">
  <span class=""preheader"">{preheader}</span>
  <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background:{brand.Bg};"">
    <tr>
      <td align=""center"" style=""padding: 32px 16px;"">
        <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" class=""container"" style=""width:600px; max-width:600px;"">
          <tr>
            <td style=""padding: 8px 0; text-align:left;"">
              <img src=""{brand.Logo}"" height=""32"" alt=""{brand.Name}"" style=""display:block; border:0; outline:none;"">
            </td>
          </tr>
          <tr>
            <td>
              <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:{brand.Card}; border-radius:16px; padding:32px;"" class=""card"">
                <tr>
                  <td style=""font-family:Arial, Helvetica, sans-serif; color:{brand.Text};"">
                    <h1 style=""margin:0 0 8px; font-size:24px; font-weight:700;"">Xác thực tài khoản</h1>
                    <p style=""margin:0 0 16px; color:{brand.Muted}; font-size:14px;"">Chào {System.Net.WebUtility.HtmlEncode(name)},</p>
                    <p style=""margin:0 0 16px; line-height:1.6;"">Đây là mã OTP để xác thực đăng ký của bạn trên <strong>{brand.Name}</strong>. 
                    Mã có hiệu lực trong <strong>{minutes} phút</strong>.</p>

                    <!-- OTP “chip” -->
                    <div style=""margin:24px 0 8px;"">
                      <code class=""otp"" style=""font-family:SFMono-Regular,Consolas,'Liberation Mono',Menlo,monospace; 
                        font-size:40px; letter-spacing:8px; display:inline-block; padding:12px 16px; 
                        color:{brand.Text}; background:linear-gradient(135deg, rgba(124,58,237,0.15), rgba(59,130,246,0.15));
                        border:1px solid rgba(124,58,237,0.35); border-radius:12px;"">{displayCode}</code>
                    </div>

                    <p style=""margin:0 0 24px; color:{brand.Muted}; font-size:13px;"">Không chia sẻ mã với bất kỳ ai. 
                    Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>

                    {(string.IsNullOrWhiteSpace(verifyUrl) ? "" : $@"
                      <div style=""margin: 16px 0 24px;"">
                        <a class=""btn"" href=""{verifyUrl}"" 
                          style=""text-decoration:none; background:{brand.ButtonBg}; color:{brand.ButtonText}; 
                          padding:12px 18px; border-radius:10px; font-weight:600; display:inline-block;"">
                          Mở trang xác thực
                        </a>
                      </div>
                    ")}

                    <hr style=""border:none; border-top:1px solid rgba(255,255,255,0.08); margin:24px 0;""/>

                    <p style=""margin:0 0 8px; font-size:12px; color:{brand.Muted};"">Nếu nút không hoạt động, vào trang:
                      <span style=""word-break:break-all;"">{(string.IsNullOrWhiteSpace(verifyUrl) ? "Trang Nhập OTP" : System.Net.WebUtility.HtmlEncode(verifyUrl))}</span>
                    </p>
                    <p style=""margin:0; font-size:12px; color:{brand.Muted};"">&copy; {DateTime.UtcNow:yyyy} {brand.Name}. All rights reserved.</p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr><td style=""height:24px;""></td></tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
            return html;
        }
    }
}