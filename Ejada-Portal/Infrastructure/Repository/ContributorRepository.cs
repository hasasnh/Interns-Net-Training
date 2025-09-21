using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public class ContributorRepository : IContributorRepository
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<Contributor> ContributorSet;
        public ContributorRepository(ApplicationDbContext db)
        {
            _db = db;
            ContributorSet = _db.Set<Contributor>();
            
        }
        public void Create(Contributor contributor)
        {
            ContributorSet.Add(contributor);
        }

        public void Delete(Contributor contributor)
        {
            ContributorSet.Remove(contributor);
        }

        public List<Contributor> GetAll()
        {
            IQueryable<Contributor> query = ContributorSet;
            return query.ToList();
        }

        public Contributor Get(Expression<Func<Contributor, bool>> filter)
        {
            IQueryable<Contributor> query = ContributorSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public void Update(Contributor contributor)
        {
            ContributorSet.Update(contributor);
        }
    }
}