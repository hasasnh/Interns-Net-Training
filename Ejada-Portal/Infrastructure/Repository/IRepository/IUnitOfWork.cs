namespace Infrastructure.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IUserRepository User { get; }
        IContributorRepository Contributor { get; }
        public void Save();
    }
}