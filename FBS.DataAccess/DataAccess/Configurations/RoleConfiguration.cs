// <copyright file= RoleConfiguration.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.DataAccess.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using FBS.Infrastructure.Entities;

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(role => role.Id);
            builder.HasIndex(role => role.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
            builder.Property(role => role.ConcurrencyStamp).IsConcurrencyToken();
            builder.Property(role => role.Name).HasMaxLength(256);
            builder.Property(role => role.NormalizedName).HasMaxLength(256);
        }
    }
}
