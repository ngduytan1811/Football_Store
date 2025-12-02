using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBS.Infrastructure.Entities
{
    [Table("ProductImages")]
    public class ProductImage : BaseModel
    {
        public Guid ProductId { get; set; }

        public string ImagePath { get; set; } = null!;

        // Navigation
        public virtual Product Product { get; set; } = null!;
    }
}
