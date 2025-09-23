using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.IServices
{
    public interface IAssignRolesService
    {
        Task<IEnumerable<UserRolesDto>> GetUsersAsync();

        Task<IEnumerable<string>> GetAllRolesAsync();

        Task<IEnumerable<string>> GetAllPagesAsync();

        Task SaveRolesAsync(List<UserRolesDto> usersRoles);
    }
}
