// <copyright file= SUMDbContext.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.DataAccess.Contexts
{
    using System;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using FBS.DataAccess.Configurations;
    using FBS.Infrastructure.Entities;
    using Microsoft.Extensions.Configuration;

    public class FBSDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public static readonly ILoggerFactory DbContextLoggerFactory = LoggerFactory.Create(builder => { builder.AddDebug(); });

        public FBSDbContext()
        {
        }

        public FBSDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", true, true);

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 30));

            var connectionString = builder.Build().GetConnectionString("DefaultConnection");
            optionsBuilder.UseMySql(connectionString, serverVersion, mySqlOptions =>
            {
                mySqlOptions.EnableStringComparisonTranslations();
                mySqlOptions.CommandTimeout(60);
            });

            optionsBuilder.UseLoggerFactory(DbContextLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new CustomerConfiguration());
            builder.ApplyConfiguration(new ContactConfiguration());
            builder.ApplyConfiguration(new MemberConfiguration());
            builder.ApplyConfiguration(new RoleClaimConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new SystemLogConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserLoginConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
            builder.ApplyConfiguration(new UserTokenConfiguration());
            builder.ApplyConfiguration(new OrderConfiguration());
            builder.ApplyConfiguration(new OrderItemConfiguration());
            builder.ApplyConfiguration(new ProductConfiguration());
            builder.ApplyConfiguration(new ProductSizeConfiguration());
            builder.ApplyConfiguration(new ProductColorConfiguration());
            builder.ApplyConfiguration(new ProductReviewConfiguration());
        }
    }
}
