using Application.Services.IServices;


namespace Application.ServiceManager
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
        IAssignRolesService AssignRolesService { get; }
        ISessionService SessionService { get; }
    }

}
