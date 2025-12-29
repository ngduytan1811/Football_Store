namespace FBS.Infrastructure.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using FBS.Infrastructure.Entities;
    using Microsoft.EntityFrameworkCore.Query;

    public interface IRepositoryAsync<T> : IDisposable
        where T : class
    {
        Task Add(T entity);

        Task Add(params T[] entities);

        Task Add(IEnumerable<T> entities);

        Task Update(T entity);

        Task Update(params T[] entities);

        Task Update(IEnumerable<T> entities);

        Task Delete(object id);

        Task Delete(T entity);

        Task Delete(params T[] entities);

        Task Delete(IEnumerable<T> entities);

        Task<T> FindById(Guid id);

        Task<T> Single(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool disableTracking = true);

        Task<IQueryable<T>> QueryCondition(Expression<Func<T, bool>> expression);
        Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        
    }
}