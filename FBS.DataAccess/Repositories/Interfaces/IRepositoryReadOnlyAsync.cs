namespace FBS.Infrastructure.Repositories.Interfaces
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Query;

    public interface IRepositoryReadOnlyAsync<T>
        where T : class
    {
        Task<T> Search(params object[] keyValues);

        Task<T> Single(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool disableTracking = true);

        Task<IQueryable<T>> QueryAll();

        Task<IQueryable<T>> QueryCondition(Expression<Func<T, bool>> expression);

        Task<IQueryable<T>> QueryRaw(string sql, params object[] parameters);

        Task<bool> Any(Expression<Func<T, bool>> expression);

        Task<IQueryable<TType>> Select<TType>(Expression<Func<T, TType>> select);
    }
}