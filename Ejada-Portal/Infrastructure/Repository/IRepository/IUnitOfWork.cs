namespace Infrastructure.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IUserRepository User { get; }
        ISessionRepository Session { get; }
        public void Save();
    }
}
