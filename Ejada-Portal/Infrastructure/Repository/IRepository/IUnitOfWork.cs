namespace Infrastructure.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IUserRepository User { get; }
        public void Save();
    }
}
