using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Infrastructure.Entities
{
    [Table("Products")]
    public class Product : BaseModel
    {
        public Guid? CategoryId { get; set; }

        public  virtual Category? Category { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        public string? Image { get; set; }
        public decimal? Price { get; set; }
        public int? Discount { get; set; }
        public string? Brand { get; set; }


        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();


        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    }
}
