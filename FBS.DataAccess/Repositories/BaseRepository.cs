namespace FBS.Infrastructure.Repositories
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public abstract class BaseRepository<T>
        where T : class
    {
        protected BaseRepository(DbContext context)
        {
            DbContext = context ?? throw new ArgumentException(nameof(context));
            DbSet = DbContext.Set<T>();
        }

        public DbSet<T> DbSet { get; }

        public DbContext DbContext { get; }
    }
}