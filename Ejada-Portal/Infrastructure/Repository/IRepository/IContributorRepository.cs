using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Repository.IRepository
{
    public interface IContributorRepository
    {
        List<Contributor> GetAll();
        Contributor Get(Expression<Func<Contributor, bool>> filter); // Expression<Func<Contributor, bool>> lets you pass LINQ expressions as parameters.
        void Create(Contributor contributor);
        void Update(Contributor contributor);
        void Delete(Contributor contributor);
    }
}
