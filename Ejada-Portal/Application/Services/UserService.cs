using Application.DTOs;
using Application.ServiceManager;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;

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

            // If the user has a saved provider and we do not choose other
            if (string.IsNullOrWhiteSpace(providerName))
            {
                providerName = string.IsNullOrWhiteSpace(user.PreferredEmailProvider) ? "Gmail" : user.PreferredEmailProvider;
            }

            var tokens = await BuildEmailTokens(user, baseResetUrl);
            var body = await _template.RenderAsync("ResetPassword.html", tokens);

            var provider = _resolver.Get(providerName);
            await provider.SendAsync(user.Email!, $"Reset Password - {tokens["DisplayName"]}", body);

            return true;
        }

        // Save the user's preferred provider in DB
        public async Task SetPreferredProviderAsync(User user, string providerName)
        {
            user.PreferredEmailProvider = providerName;
            await _userManager.UpdateAsync(user);
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
        // Fetch the current user
        public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }
        //Update any changes on user data
        public async Task UpdateUserAsync(User user)
        {
            await _userManager.UpdateAsync(user);
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
    }
}
