using Infrastructure.Data;
using Infrastructure.Repository.IRepository;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IUserRepository User { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            User = new UserRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
