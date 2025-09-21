using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Services.IServices
{
    public interface IUserService
    {
        UserDTO MapToDTO(User user);
        User MapToUser(UserDTO userDTO);
        User CheckUser(string Email, string username);
        Task Login(User user);
        Task<IdentityResult> Register(User user, string password);
        Task<SignInResult> CheckPassword(string userName, string password);
        Task SignOut();

        Task<bool> SendPasswordResetLinkAsync(string email, string baseResetUrl);
        Task<bool> SendPasswordResetLinkAsync(string email, string baseResetUrl, string providerName);

        Task<IdentityResult> ResetPasswordAsync(string email, string tokenEnc, string newPassword);
        Task<IdentityResult> ResetPasswordAsync(string email, string tokenEnc, string newPassword, string providerName);
        Task SetPreferredProviderAsync(User user, string selectedProvider);
        Task<User?> GetCurrentUserAsync(ClaimsPrincipal principal);
        Task UpdateUserAsync(User user);
    }
}
