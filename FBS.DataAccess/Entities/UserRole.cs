// <copyright file= UserRole.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    [Table("UserRoles")]
    public class UserRole : IdentityUserRole<Guid>
    {
        public bool IsActive { get; set; }

        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}
