using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.IServices
{
    public interface ISessionRatingService
    {
        Task<SessionRatingDto> AddRatingAsync(SessionRatingDto dto);

        Task<IEnumerable<SessionRatingDto>> GetRatingsBySessionAsync(int sessionId);

        Task<IEnumerable<SessionRatingDto>> GetRatingsByUserAsync(string userId);

        Task<IEnumerable<SessionRatingSummaryDto>> GetSessionRatingsSummaryAsync();
    }
}
