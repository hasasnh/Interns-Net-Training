using Application.Services;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using System;

namespace Application.ServiceManager
{
    public class ServiceManager : IServiceManager
    {
        public Lazy<IUserService> _userService { get; private set; }
        public Lazy<IAssignRolesService> _assignRolesService { get; private set; }
        public Lazy<ISessionService> _sessionService { get; private set; }
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IContributorService> _contributorService;
       
       public Lazy<IAssignRolesService> _assignRolesService { get; private set; }

        public ServiceManager(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            IEmailProviderResolver resolver,
            IEmailTemplateRenderer templateRenderer)
        {
            _userService = new Lazy<IUserService>(() => new UserService(unitOfWork, userManager, signInManager, resolver, templateRenderer));
            _assignRolesService = new Lazy<IAssignRolesService>(() => new AssignRolesService(userManager, roleManager));
            _sessionService = new Lazy<ISessionService>(() => new SessionService(unitOfWork));

            
            _contributorService = new Lazy<IContributorService>(() => new ContributorService(unitOfWork));
        }

        public IUserService UserService => _userService.Value;
        public IContributorService ContributorService => _contributorService.Value;
        public IAssignRolesService AssignRolesService => _assignRolesService.Value;
        public ISessionService SessionService => _sessionService.Value;
    }
}