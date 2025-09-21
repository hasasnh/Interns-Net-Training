using Application.Services.IServices;

namespace Application.ServiceManager
{
    public class ServiceManager : IServiceManager
    {
        public IUserService UserService { get; }
        public ServiceManager(IUserService userService) => UserService = userService;
    }
}
