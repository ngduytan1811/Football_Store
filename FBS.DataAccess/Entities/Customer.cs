// <copyright file= Customer.cs company=Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Customers")]
    public class Customer : BaseModel
    {
        public Guid? UserId { get; set; }

        public virtual User? User { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? FullName => $"{FirstName} {LastName}";

        public string PhoneNumber { get; set; }

        public string? Avatar { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        public string? Address { get; set; } = string.Empty;

        public DateTime? BirthDay { get; set; } = null;
    }
}
