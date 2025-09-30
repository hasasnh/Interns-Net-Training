using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.IServices
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionDto>> GetAllSessionsAsync();

        Task<SessionDto> GetSessionByIdAsync(int id);

        Task<SessionDto> CreateSessionAsync(SessionDto dto);

        Task<SessionDto> UpdateSessionAsync(SessionDto dto);

        Task<bool> DeleteSessionAsync(int id);



    }
}
