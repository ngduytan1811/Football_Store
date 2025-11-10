// <copyright file= Member.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Members")]
    public class Member : BaseModel
    {
        public Guid? UserId { get; set; }

        public virtual User? User { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Avatar { get; set; }

        public DateTime? BirthDay { get; set; }
    }
}
