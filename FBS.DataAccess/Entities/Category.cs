// <copyright file= Category.cs company= Giang Nguyen>
// Copyright (c) Giang Nguyen. All rights reserved.
// </copyright>

namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Categories")]
    public class Category : BaseModel
    {
        public string? Code { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Logo { get; set; }

        public int? Order { get; set; }

        public bool IsOutstanding { get; set; } = false;
    }
}
