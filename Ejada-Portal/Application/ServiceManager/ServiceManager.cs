using Application.Services;
using Application.Services.IServices;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;

namespace Application.ServiceManager
{
    public class ServiceManager : IServiceManager
    {
        public Lazy<IUserService> _userService { get; private set; }
        public Lazy<IAssignRolesService> _assignRolesService { get; private set; }
        public Lazy<ISessionService> _sessionService { get; private set; }
        public Lazy<IJiraService> _jiraService { get; private set; }
        private readonly Lazy<IContributorService> _contributorService;
        private readonly Lazy<ISessionRatingService> _sessionRatingService;

        public ServiceManager(
            ApplicationDbContext dbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailProviderResolver resolver,
            IEmailTemplateRenderer templateRenderer,
            IHttpClientFactory factory)
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

            _jiraService = new Lazy<IJiraService>(() => new JiraService(factory));
        }

        public IUserService UserService => _userService.Value;
        public IAssignRolesService AssignRolesService => _assignRolesService.Value;
        public IContributorService ContributorService => _contributorService.Value;
        public ISessionService SessionService => _sessionService.Value;
        public ISessionRatingService SessionRatingService => _sessionRatingService.Value;
        public IJiraService JiraService => _jiraService.Value;
    }
}
