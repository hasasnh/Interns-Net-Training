using Infrastructure.Data;
using Infrastructure.Repository.IRepository;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IUserRepository User { get; private set; }
        public ISessionRepository Session { get; private set; }
        public IContributorRepository Contributor { get; private set; }
        public ISessionRatingRepository SessionRating { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            User = new UserRepository(_db);
            Session = new SessionRepository(_db);
            Contributor = new ContributorRepository(_db);
            SessionRating = new SessionRatingRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
