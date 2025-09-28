using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrastructure.Repository.IRepository
{
    public interface ISessionRepository
    {
        List<Session> GetAll();
        Session? Get(Expression<Func<Session, bool>> filter);
        void Create(Session entity);
        void Update(Session entity);
        void Delete(Session entity);
    }
}
