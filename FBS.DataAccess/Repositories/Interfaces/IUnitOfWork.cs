namespace FBS.Infrastructure.Repositories.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using static FBS.Infrastructure.Authorization.Permissions;

    public interface IUnitOfWork : IDisposable
    {
        Guid? CurrentUserEntityId { get; set; }
      

        IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>()
            where TEntity : class;

        IRepositoryReadOnlyAsync<TEntity> GetRepositoryReadOnlyAsync<TEntity>()
            where TEntity : class;

        Task<int> SaveChangesAsync();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork
        where TContext : DbContext
    {
        TContext Context { get; }
    }
}
