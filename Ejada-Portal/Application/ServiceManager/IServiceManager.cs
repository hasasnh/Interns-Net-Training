using Application.Services.IServices;

namespace Application.ServiceManager
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
        IContributorService ContributorService { get; }
        IAssignRolesService AssignRolesService { get; }
        ISessionService SessionService { get; }
        IJiraService JiraService { get; }
    }

}
