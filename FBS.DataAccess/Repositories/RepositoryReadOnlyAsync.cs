// <copyright file= RepositoryReadOnlyAsync.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Infrastructure.Repositories
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using FBS.Infrastructure.Repositories.Interfaces;

    public class RepositoryReadOnlyAsync<T> : BaseRepository<T>, IRepositoryReadOnlyAsync<T>
        where T : class
    {
        public RepositoryReadOnlyAsync(DbContext context)
            : base(context)
        {
        }

        public async Task<T> Search(params object[] keyValues) => await Task.Run(() => DbSet.Find(keyValues));

        public async Task<IQueryable<T>> QueryAll() => await Task.Run(() => DbSet.AsNoTracking());

        public Task<IQueryable<T>> QueryCondition(Expression<Func<T, bool>> expression)
        {
            return Task.FromResult(DbSet.Where(expression).AsNoTracking());
        }


        public async Task<IQueryable<T>> QueryRaw(string sql, params object[] parameters) => await Task.Run(() => DbSet.FromSqlRaw(sql, parameters));

        public async Task<bool> Any(Expression<Func<T, bool>> expression) => await Task.Run(() => DbSet.Any(expression));

        public async Task<IQueryable<TType>> Select<TType>(Expression<Func<T, TType>> select) => await Task.Run(() => DbSet.Select(select));

        public async Task<T> Single(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = DbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                return await orderBy(query).FirstOrDefaultAsync();
            }

            return await query.FirstOrDefaultAsync();
        }
    }
}