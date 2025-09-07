using Application.DTOs;
using Application.ServiceManager;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        public UserService(IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public async Task<SignInResult> CheckPassword(string userName, string password)
        {
            return await _signInManager.PasswordSignInAsync(userName, password, false, true);
        }

        public User CheckUser(string Email, string username)
        {
            return _unitOfWork.User.Get(u => u.Email == Email || u.UserName == username);
        }

        public async Task Login(User user)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        public UserDTO MapToDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };
        }

        public User MapToUser(UserDTO userDTO)
        {
            return new User
            {
                UserName = userDTO.Username,
                Email = userDTO.Email
            };
        }

        public async Task<IdentityResult> Register(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        // New

        public async Task<bool> SendPasswordResetLinkAsync(string email, string baseResetUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            // توليد التوكن وترميزه
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenEnc = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var fullLink = $"{baseResetUrl}?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(tokenEnc)}";

            var displayName = !string.IsNullOrWhiteSpace(user.Name) ? user.Name : (user.UserName ?? user.Email!.Split('@')[0]);

            var requestUtc = DateTime.UtcNow; // وقت الطلب
            var requestLocal = requestUtc.ToLocalTime();

            //  موضوع الرسالة باسم المستخدم
            var subject = $"إعادة تعيين كلمة المرور - {displayName}";

            var body = $@"<!doctype html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <title>Password Reset</title>
</head>
<body dir=""ltr"" style=""font-family:Tahoma,Arial,sans-serif; background:#f7f7f7; margin:0; padding:20px;"">
  <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" align=""center"" width=""100%"" style=""max-width:600px; background:#ffffff; border-radius:8px; overflow:hidden;"">
    <tr>
      <td style=""padding:24px 24px 0 24px;"">
        <h2 style=""margin:0 0 12px 0; color:#333;"">Hello {System.Net.WebUtility.HtmlEncode(displayName)},</h2>
        <p style=""margin:0 0 8px 0; color:#555;"">
          You requested to reset your account password.
        </p>
        <p style=""margin:0 0 16px 0; color:#555;"">
          <strong>Email:</strong> {System.Net.WebUtility.HtmlEncode(user.Email!)}<br/>
          <strong>Request time:</strong> {requestLocal:yyyy/MM/dd HH:mm}
        </p>
      </td>
    </tr>
    <tr>
      <td style=""padding:0 24px 24px 24px;"">
        <p style=""margin:0 0 16px 0; color:#555;"">Click the button below to reset your password:</p>
        <p style=""margin:0 0 24px 0;"">
          <a href=""{fullLink}"" style=""display:inline-block; background:#0d6efd; color:#fff; text-decoration:none; padding:12px 18px; border-radius:6px;"">Reset password now</a>
        </p>
        <p style=""margin:0 0 8px 0; color:#777; font-size:13px;"">
          If you didn't request this, you can safely ignore this email. The link will expire after a limited time.
        </p>
        
      </td>
    </tr>
  </table>
</body>
</html>";


            await _emailSender.SendAsync(user.Email!, subject, body);
            return true;

        }


        public async Task<IdentityResult> ResetPasswordAsync(string email, string tokenEnc, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "المستخدم غير موجود." });

            var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(tokenEnc));
            return await _userManager.ResetPasswordAsync(user, decoded, newPassword);
        }
    }
}

