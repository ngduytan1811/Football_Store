// <copyright file= MemberConfiguration.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.DataAccess.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using FBS.Infrastructure.Entities;
    using FBS.Shared.Enums;

    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.HasData(
               new Member
               {
                   Id = new Guid("1b2835d1-3468-4ad0-8881-ca52cdf1307d"),
                   UserId = new Guid("0b2863d1-3468-4ad0-8881-ca52cdf1307d"),
                   FirstName = "admin",
                   Status = StatusEnum.Active,
               });
        }
    }
}
