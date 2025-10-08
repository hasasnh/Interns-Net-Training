using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _db;
        public SessionRepository(ApplicationDbContext db) => _db = db;

        public async Task<Session> GetByIdAsync(int id) => await _db.Sessions.FindAsync(id);
        public async Task<IEnumerable<Session>> GetAllAsync() => await _db.Sessions.ToListAsync();
        public async Task AddAsync(Session session)
        {
            await _db.Sessions.AddAsync(session);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateAsync(Session session)
        {
            _db.Sessions.Update(session);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var s = await _db.Sessions.FindAsync(id);
            if (s != null) _db.Sessions.Remove(s);
            await _db.SaveChangesAsync();
        }
    }
}
