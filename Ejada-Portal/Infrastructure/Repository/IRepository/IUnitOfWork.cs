namespace Infrastructure.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IUserRepository User { get; }
        ISessionRepository Session { get; }

        IContributorRepository Contributor { get; }
        public void Save();
    }
}