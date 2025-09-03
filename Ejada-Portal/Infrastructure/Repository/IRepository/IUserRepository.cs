using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Repository.IRepository
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User Get(Expression<Func<User, bool>> filter);
        void Create(User user);
        void Update(User user);
        void Delete(User user);
    }
}
