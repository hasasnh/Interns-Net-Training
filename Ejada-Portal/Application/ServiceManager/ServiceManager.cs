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
        public  Lazy<IUserService> _userService { get; private set; }
        public Lazy<Services.IServices.IAssignRolesService> _assignRolesService { get; private set; }

        public ServiceManager(IUnitOfWork unitOfWork,
                              UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              RoleManager<IdentityRole> roleManager
                           )
        {
            _userService = new Lazy<IUserService>(() => new UserService(unitOfWork, userManager, signInManager));

            _assignRolesService = new Lazy<Services.IServices.IAssignRolesService>(() => new AssignRolesService( userManager, roleManager));

        }
        public IUserService UserService => _userService.Value;

        public IAssignRolesService AssignRolesService => _assignRolesService.Value;
    }
}
