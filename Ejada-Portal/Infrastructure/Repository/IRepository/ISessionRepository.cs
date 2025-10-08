using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrastructure.Repository.IRepository
{
    public interface ISessionRepository
    {
        Task<Session> GetByIdAsync(int id);
        Task<IEnumerable<Session>> GetAllAsync();
        Task AddAsync(Session session);
        Task UpdateAsync(Session session);
        Task DeleteAsync(int id);
    }
}
