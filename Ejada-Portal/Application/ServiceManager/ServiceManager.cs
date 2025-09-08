using Application.Services;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace Application.ServiceManager
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _userService;

        public ServiceManager(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ApplicationDbContext db,
            IEmailSender emailSender,
            IEmailTemplateRenderer templateRenderer) 
        {
            _userService = new Lazy<IUserService>(
                () => new UserService(unitOfWork, userManager, signInManager, emailSender,templateRenderer)
            );
        }

        public IUserService UserService => _userService.Value;
    }
}
