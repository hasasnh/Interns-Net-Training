using Application.Services;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository;
using Infrastructure.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using System;

namespace Application.ServiceManager
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IAssignRolesService> _assignRolesService;
        private readonly Lazy<IContributorService> _contributorService;
        private readonly Lazy<ISessionService> _sessionService;
        private readonly Lazy<ISessionRatingService> _sessionRatingService;

        public ServiceManager(
            ApplicationDbContext dbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailProviderResolver resolver,
            IEmailTemplateRenderer templateRenderer)
        {
            var unitOfWork = new UnitOfWork(dbContext);

            _userService = new Lazy<IUserService>(() =>
                new UserService(unitOfWork, userManager, signInManager, resolver, templateRenderer));

            _assignRolesService = new Lazy<IAssignRolesService>(() =>
                new AssignRolesService(userManager, roleManager));

            _contributorService = new Lazy<IContributorService>(() =>
                new ContributorService(unitOfWork));

            _sessionService = new Lazy<ISessionService>(() =>
             new SessionService(unitOfWork)); 


            _sessionRatingService = new Lazy<ISessionRatingService>(() =>
                new SessionRatingService(unitOfWork.SessionRating, unitOfWork.Session));
        }

        public IUserService UserService => _userService.Value;
        public IAssignRolesService AssignRolesService => _assignRolesService.Value;
        public IContributorService ContributorService => _contributorService.Value;
        public ISessionService SessionService => _sessionService.Value;
        public ISessionRatingService SessionRatingService => _sessionRatingService.Value;
    }
}
