using Infrastructure.Data;
using Infrastructure.Repository.IRepository;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IUserRepository User { get; private set; }
        public ISessionRepository Session { get; private set; }  //new

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            User = new UserRepository(_db);
            Session = new SessionRepository(_db);//new
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
