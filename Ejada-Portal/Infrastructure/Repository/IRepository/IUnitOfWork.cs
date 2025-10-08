using System.Threading.Tasks;

namespace Infrastructure.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IUserRepository User { get; }
        ISessionRepository Session { get; }
        IContributorRepository Contributor { get; }
        ISessionRatingRepository SessionRating { get; }

        void Save(); 
        Task<int> SaveAsync(); 
    }
}
