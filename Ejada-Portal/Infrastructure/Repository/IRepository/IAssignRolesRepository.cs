using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.IRepository
{
    public interface IAssignRolesRepository
    {
        Task<IEnumerable<Domain.Entities.User>> GetUsersAsync();
        Task<IEnumerable<string>> GetAllRolesAsync();
        Task<IEnumerable<Domain.Entities.PagePermission>> GetAllPagesAsync();
        Task SaveRolesAsync(List<Domain.Entities.User> usersRoles);
    }
}
