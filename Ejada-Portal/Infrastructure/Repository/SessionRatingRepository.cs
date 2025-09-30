using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class SessionRatingRepository : ISessionRatingRepository
    {
        private readonly ApplicationDbContext _db;
        public SessionRatingRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(SessionRating rating)
        {
            await _db.SessionRatings.AddAsync(rating);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<SessionRating>> GetAllAsync()
        {
            return await _db.SessionRatings.Include(r => r.Session).ToListAsync();
        }

        public async Task<IEnumerable<SessionRating>> GetByUserIdAsync(string userId)
        {
            return await _db.SessionRatings
                .Where(r => r.UserId == userId)
                .Include(r => r.Session)
                .ToListAsync();
        }

        public async Task<IEnumerable<SessionRating>> GetBySessionIdAsync(int sessionId)
        {
            return await _db.SessionRatings
                .Where(r => r.SessionId == sessionId)
                .Include(r => r.Session)
                .ToListAsync();
        }
    }
}
