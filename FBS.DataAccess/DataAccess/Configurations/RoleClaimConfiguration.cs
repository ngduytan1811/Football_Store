// <copyright file= RoleClaimConfiguration.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.DataAccess.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using FBS.Infrastructure.Entities;

    public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.HasKey(roleClaim => roleClaim.Id);

            builder
                .HasOne(roleClaim => roleClaim.Role)
                .WithMany(role => role.RoleClaims)
                .HasForeignKey(roleClaim => roleClaim.RoleId);
        }
    }
}
