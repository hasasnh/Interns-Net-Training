using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

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
    }
}
