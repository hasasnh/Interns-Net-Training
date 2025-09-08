using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs;
using Application.ServiceManager;                
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateRenderer _template;   // ✅ جديد

        public UserService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            IEmailTemplateRenderer templateRenderer)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _template = templateRenderer;
        }

        public async Task<SignInResult> CheckPassword(string userName, string password)
            => await _signInManager.PasswordSignInAsync(userName, password, false, true);

        public User CheckUser(string Email, string username)
            => _unitOfWork.User.Get(u => u.Email == Email || u.UserName == username);

        public async Task Login(User user)
            => await _signInManager.SignInAsync(user, isPersistent: false);

        public UserDTO MapToDTO(User user)
            => new UserDTO
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };

        public User MapToUser(UserDTO userDTO)
            => new User
            {
                UserName = userDTO.Username,
                Email = userDTO.Email
            };

        public async Task<IdentityResult> Register(User user, string password)
            => await _userManager.CreateAsync(user, password);

        public async Task SignOut()
            => await _signInManager.SignOutAsync();

        //New Code --Mohammad Musatfa--
        public async Task<bool> SendPasswordResetLinkAsync(string email, string baseResetUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            // توليد التوكن وترميزه Base64Url
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenEnc = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // بناء الرابط الكامل
            var fullLink =
                $"{baseResetUrl}?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(tokenEnc)}";

            // اسم العرض
            var displayName = !string.IsNullOrWhiteSpace(user.Name)
                ? user.Name
                : (user.UserName ?? user.Email!.Split('@')[0]);

            var requestLocal = DateTime.UtcNow.ToLocalTime();

            // موضوع الرسالة
            var subject = $"إعادة تعيين كلمة المرور - {displayName}";

            // توكنات القالب
            var tokens = new Dictionary<string, string>
            {
                ["DisplayName"] = System.Net.WebUtility.HtmlEncode(displayName),
                ["Email"] = System.Net.WebUtility.HtmlEncode(user.Email!),
                ["RequestTime"] = requestLocal.ToString("yyyy/MM/dd HH:mm"),
                ["ResetLink"] = fullLink
            };

            // 🔹 قراءة القالب وحقن القيم
            var body = await _template.RenderAsync("ResetPassword.html", tokens);

            // إرسال الرسالة
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
