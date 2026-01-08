using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using FBS.Infrastructure.Repositories.Interfaces;
using FBS.Shared.Constants;

namespace FBS.Infrastructure.Repositories
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
        where TContext : DbContext, IDisposable
    {
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UnitOfWork(TContext context, IDistributedCache distributedCache, IMemoryCache memoryCache)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            DistributedCache = distributedCache;
            MemoryCache = memoryCache;
        }

        public TContext Context { get; }

        public Guid? CurrentUserEntityId { get; set; }

        public IMemoryCache MemoryCache { get; set; }

        public IDistributedCache DistributedCache { get; set; }

        
        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>()
            where TEntity : class
        {
            var type = typeof(IRepositoryAsync<TEntity>);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new RepositoryAsync<TEntity>(Context);
            }

            return (IRepositoryAsync<TEntity>)_repositories[type];
        }

        // -------------------------
        // READONLY REPOSITORY
        // -------------------------
        public IRepositoryReadOnlyAsync<TEntity> GetRepositoryReadOnlyAsync<TEntity>()
            where TEntity : class
        {
            var type = typeof(IRepositoryReadOnlyAsync<TEntity>);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new RepositoryReadOnlyAsync<TEntity>(Context);
            }

            return (IRepositoryReadOnlyAsync<TEntity>)_repositories[type];
        }

        // -------------------------
        // SAVE
        // -------------------------
        public async Task<int> SaveChangesAsync()
        {
            SaveChangesInternal();
            return await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context?.Dispose();
            GC.SuppressFinalize(this);
        }

        // -------------------------
        // INTERNAL SAVE LOGIC
        // -------------------------
        private void SaveChangesInternal()
        {
            Context.ChangeTracker.DetectChanges();
            var entries = Context.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            SaveChangesInternal(entries, EntityState.Added);
            SaveChangesInternal(entries, EntityState.Modified);
        }

        private void SaveChangesInternal(IEnumerable<EntityEntry> entries, EntityState state)
        {
            PropertyEntry prop;

            foreach (var item in entries)
            {
                foreach (var p in item.Properties)
                {
                    if (p.CurrentValue == null)
                        continue;

                    if (p.Metadata.ClrType == typeof(string))
                    {
                        var emptyString = string.IsNullOrWhiteSpace(p.CurrentValue.ToString());
                        p.CurrentValue = emptyString ? null : p.CurrentValue;
                    }
                }
            }

            foreach (var item in entries.Where(t => t.State == state))
            {
                if (state == EntityState.Added)
                {
                    prop = item.Properties.FirstOrDefault(p => p.Metadata.Name == ColumnNames.CreatedAt);
                    if (prop != null)
                    {
                        prop.CurrentValue = DateTime.Now;

                        if (CurrentUserEntityId != null)
                        {
                            prop = item.Properties.FirstOrDefault(p => p.Metadata.Name == ColumnNames.CreatedById);
                            if (prop != null)
                                prop.CurrentValue = CurrentUserEntityId;
                        }
                    }
                }

                prop = item.Properties.FirstOrDefault(p => p.Metadata.Name == ColumnNames.UpdatedAt);
                if (prop != null)
                {
                    prop.CurrentValue = DateTime.Now;

                    if (CurrentUserEntityId != null)
                    {
                        prop = item.Properties.FirstOrDefault(p => p.Metadata.Name == ColumnNames.UpdatedById);
                        if (prop != null)
                            prop.CurrentValue = CurrentUserEntityId;
                    }
                }

                var stringProps = item.Properties
                    .Where(p => p.CurrentValue != null && p.CurrentValue is string);

                foreach (var sp in stringProps)
                {
                    sp.CurrentValue = sp.CurrentValue.ToString().Trim();
                }
            }
        }
    }
}
