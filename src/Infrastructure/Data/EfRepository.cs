using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class EfRepository<T> : IRepository<T>, IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly CurrencyRateContext dbContext;

        public EfRepository(CurrencyRateContext dbContext) => this.dbContext = dbContext;

        public virtual T GetById(int id) => dbContext.Set<T>().Find(id);

        public T GetSingleBySpec(ISpecification<T> spec) => List(spec).FirstOrDefault();

        public virtual async Task<T> GetByIdAsync(int id) => await dbContext.Set<T>().FindAsync(id);

        public IEnumerable<T> ListAll() => dbContext.Set<T>().AsEnumerable();

        public async Task<List<T>> ListAllAsync() => await dbContext.Set<T>().ToListAsync();

        public IEnumerable<T> List(ISpecification<T> spec)
        {
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(dbContext.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            return secondaryResult
                .Where(spec.Criteria)
                .AsEnumerable();
        }
        public async Task<List<T>> ListAsync(ISpecification<T> spec)
        {
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(dbContext.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            return await secondaryResult
                .Where(spec.Criteria)
                .ToListAsync();
        }

        public T Add(T entity)
        {
            dbContext.Set<T>().Add(entity);
            dbContext.SaveChanges();

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            dbContext.Set<T>().Add(entity);
            await dbContext.SaveChangesAsync();

            return entity;
        }

        public void Update(T entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
        public async Task UpdateAsync(T entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            dbContext.SaveChanges();
        }
        public async Task DeleteAsync(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
