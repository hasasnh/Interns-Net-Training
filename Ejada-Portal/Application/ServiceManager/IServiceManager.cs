using Application.Services.IServices;

namespace Application.ServiceManager
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
        IAssignRolesService AssignRolesService { get; }
        IContributorService ContributorService { get; }
        ISessionService SessionService { get; }
        ISessionRatingService SessionRatingService { get; } 
    }

}
