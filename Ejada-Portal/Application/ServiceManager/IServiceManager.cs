using Application.Services.IServices;


namespace Application.ServiceManager
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
    }
}
