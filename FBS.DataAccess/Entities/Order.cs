// <copyright file= Order.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Orders")]
    public class Order : BaseModel
    {
        public Guid? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerEmail { get; set; }

        public string? CustomerAddress { get; set; }

        public string? Note { get; set; }
    }
}
