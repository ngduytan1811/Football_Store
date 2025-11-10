// <copyright file= UserToken.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    [Table("UserTokens")]
    public class UserToken : IdentityUserToken<Guid>
    {
        public virtual User User { get; set; }
    }
}
