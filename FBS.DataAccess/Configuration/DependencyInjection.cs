// <copyright file= DependencyInjection.cs company=Tan Nguyen>
// Copyright (c) Tan Nguyen. All rights reserved.
// </copyright>

namespace FBS.Infrastructure.Configuration
{
    using System.Reflection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Impl;
    using FBS.DataAccess.Contexts;
    using FBS.Infrastructure.Repositories;
    using FBS.Infrastructure.Repositories.Interfaces;
    using FBS.Shared.Constants;
    using FBS.Infrastructure.Entities;

    public static class DependencyInjection
    {
        public static void SetDBContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = configuration.GetConnectionString("DefaultConnection");
            }

            MySqlServerVersion serverVersion;
            var sqlVersion = Environment.GetEnvironmentVariable("MYSQL_VERSION");
            if (string.IsNullOrEmpty(sqlVersion))
            {
                var major = int.Parse(configuration["MySqlServerVersion:Major"]);
                var minor = int.Parse(configuration["MySqlServerVersion:Minor"]);
                var build = int.Parse(configuration["MySqlServerVersion:Build"]);

                serverVersion = new MySqlServerVersion(new System.Version(major, minor, build));
            }
            else
            {
                serverVersion = new MySqlServerVersion(new System.Version(sqlVersion));
            }

            services.AddDbContext<FBSDbContext>(options =>
               options.UseMySql(connectionString, serverVersion, builder => builder.MigrationsAssembly("FBS.Infrastructure")));
        }

        public static IServiceCollection InitialApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork<FBSDbContext>>();

            RegisterServiceInterfaces(services);

            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            RegisterCacheInterfaces(services, configuration);

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
            }).AddEntityFrameworkStores<FBSDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Admin/Auth/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            return services;
        }

        public static void InitialDBAndSeedData(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                try
                {
                    var initialiser = scope.ServiceProvider.GetRequiredService<FBSDbContext>();
                    initialiser.Database.Migrate();
                }
                catch (Exception ex)
                {
                    throw new InvalidDataException(ex.Message);
                }
            }
        }

        private static void RegisterCacheInterfaces(IServiceCollection services, IConfiguration configuration)
        {
            var redisConfiguration = configuration.GetSection("RedisCache:Configuration").Value;
            var redisInstanceName = configuration.GetSection("RedisCache:InstanceName").Value;
            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = string.IsNullOrWhiteSpace(redisConfiguration) ? CacheSettings.DistributedRedisConfiguration : redisConfiguration;
                option.InstanceName = string.IsNullOrWhiteSpace(redisInstanceName) ? CacheSettings.DistributedRedisInstanceName : redisInstanceName;
            });
        }

        private static void RegisterServiceInterfaces(IServiceCollection services)
        {
            var externalAssembly = Assembly.Load("FBS.Application");

            var types = externalAssembly.GetTypes();

            foreach (var implementationType in types
                .Where(a => a.Name.EndsWith("Service") && !a.IsAbstract && !a.IsInterface))
            {
                var interfaces = implementationType.GetInterfaces();

                foreach (var @interface in interfaces)
                {
                    services.AddScoped(@interface, implementationType);
                }
            }
        }
    }
}
