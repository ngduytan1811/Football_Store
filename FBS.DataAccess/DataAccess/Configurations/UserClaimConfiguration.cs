// <copyright file= UserClaimConfiguration.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.DataAccess.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using FBS.Infrastructure.Entities;

    public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.HasKey(userClaim => userClaim.Id);

            builder
                .HasOne(userClaim => userClaim.User)
                .WithMany(user => user.UserClaims)
                .HasForeignKey(userClaim => userClaim.UserId);
        }
    }
}
