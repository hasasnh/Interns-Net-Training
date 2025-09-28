using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.IServices
{
    public interface ISessionService
    {
        SessionDto MapToDto(Domain.Entities.Session entity);
        Domain.Entities.Session MapToEntity(SessionDto dto);

        Task AddAsync(SessionDto dto);
        Task<IEnumerable<SessionDto>> GetAllAsync();
    }
}
