// <copyright file= UserConfiguration.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.DataAccess.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using FBS.Infrastructure.Entities;
    using FBS.Shared.Enums;
    using FBS.Shared.Utilities;

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id);
            builder.HasIndex(user => user.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
            builder.HasIndex(user => user.NormalizedEmail).HasDatabaseName("EmailIndex");
            builder.Property(user => user.ConcurrencyStamp).IsConcurrencyToken();
            builder.Property(user => user.UserName).HasMaxLength(256);
            builder.Property(user => user.NormalizedUserName).HasMaxLength(256);
            builder.Property(user => user.Email).HasMaxLength(256);
            builder.Property(user => user.NormalizedEmail).HasMaxLength(256);

            builder.HasData(
               new User
               {
                   Id = new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                   UserName = "admin",
                   PasswordHash = PasswordUtils.HashPassword("123"),
                   LockoutEnabled = true,
                   ConcurrencyStamp = "616f1653-48e9-4a6f-81b3-1bdd52e565b5",
                   NormalizedUserName = "ADMIN",
                   IsAdmin = true,
                   SecurityStamp = "ZY5BGSWBARTE74T6ZLO7WKKMMILBEB2E",
                   Status = StatusEnum.Active,
               });
        }
    }
}
