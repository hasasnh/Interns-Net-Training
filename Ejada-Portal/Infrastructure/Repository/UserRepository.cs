using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<User> userSet;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
            userSet = _db.Set<User>();
        }
        public void Create(User user)
        {
            userSet.Add(user);
        }

        public void Delete(User user)
        {
            userSet.Remove(user);
        }

        public List<User> GetAll()
        {
            IQueryable<User> query = userSet;
            return query.ToList();
        }

        public User Get(Expression<Func<User, bool>> filter)
        {
            IQueryable<User> query = userSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public void Update(User user)
        {
            userSet.Update(user);
        }
    }
}
