// <copyright file= RoleClaim.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    [Table("RoleClaims")]
    public class RoleClaim : IdentityRoleClaim<Guid>
    {
        public string? FeatureName { get; set; }

        public virtual Role Role { get; set; }
    }
}
