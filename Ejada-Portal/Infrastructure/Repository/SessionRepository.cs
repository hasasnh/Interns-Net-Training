using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<Session> set;

        public SessionRepository(ApplicationDbContext db)
        {
            _db = db;
            set = _db.Set<Session>();
        }

        public void Create(Session entity) => set.Add(entity);

        public void Delete(Session entity) => set.Remove(entity);

        public Session? Get(Expression<Func<Session, bool>> filter)
            => set.Where(filter).FirstOrDefault();

        public List<Session> GetAll() => set.OrderByDescending(s => s.CreatedAt).ToList();

        public void Update(Session entity) => set.Update(entity);
    }
}
