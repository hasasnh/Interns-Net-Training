using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISessionRatingRepository
{
    Task AddAsync(SessionRating rating);
    Task<IEnumerable<SessionRating>> GetByUserIdAsync(string userId);
    Task<IEnumerable<SessionRating>> GetBySessionIdAsync(int sessionId);
    Task<IEnumerable<SessionRating>> GetAllAsync();
}
