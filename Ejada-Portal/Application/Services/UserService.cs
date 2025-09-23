using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
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
        private readonly IEmailProviderResolver _resolver;
        private readonly IEmailTemplateRenderer _template;

        public UserService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailProviderResolver resolver,
            IEmailTemplateRenderer templateRenderer)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _template = templateRenderer ?? throw new ArgumentNullException(nameof(templateRenderer));
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

        // -- Mohammad Mustafa --

        //If a provider name is not provided, it defaults to Gmail
        public async Task<bool> SendPasswordResetLinkAsync(string email, string baseResetUrl)
        {
            return await SendPasswordResetLinkAsync(email, baseResetUrl, "Gmail");
        }


        public async Task<bool> SendPasswordResetLinkAsync(string email, string baseResetUrl, string providerName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var tokens = await BuildEmailTokens(user, baseResetUrl);
            var body = await _template.RenderAsync("ResetPassword.html", tokens);
            var subject = $"Reset Password - {tokens["DisplayName"]}";

            IEmailProvider provider;
            try
            {
                provider = _resolver.Get(string.IsNullOrWhiteSpace(providerName) ? "Gmail" : providerName);
            }
            catch
            {
                provider = _resolver.Get("Gmail");
            }

            await provider.SendAsync(user.Email!, subject, body);
            return true;
        }

        // If a provider name is not provided, it defaults to Gmail  
       public async Task<IdentityResult> ResetPasswordAsync(string email, string tokenEnc, string newPassword)
        {
            return await ResetPasswordAsync(email, tokenEnc, newPassword, "Gmail");
        }
     

        public async Task<IdentityResult> ResetPasswordAsync(string email, string tokenEnc, string newPassword, string providerName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(tokenEnc));
            var result = await _userManager.ResetPasswordAsync(user, decoded, newPassword);
            if (!result.Succeeded) return result;

            return result;
        }

   
        private async Task<Dictionary<string, string>> BuildEmailTokens(User user, string baseResetUrl)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenEnc = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var fullLink = $"{baseResetUrl}?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(tokenEnc)}";

            var displayName = !string.IsNullOrWhiteSpace(user.Name)
                ? user.Name
                : (user.UserName ?? user.Email!.Split('@')[0]);

            return new Dictionary<string, string>
            {
                ["DisplayName"] = displayName,
                ["Email"] = user.Email ?? "",
                ["RequestTime"] = DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mm"),
                ["ResetLink"] = fullLink
            };
        }


        // add user to role
        public async Task AssignUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
