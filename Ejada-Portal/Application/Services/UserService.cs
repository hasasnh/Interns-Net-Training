using Application.DTOs;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserService(IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
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
    }
}
