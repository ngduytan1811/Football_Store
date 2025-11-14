using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Infrastructure.Entities
{
    [Table("ProductReviews")]
    public class ProductReview : BaseModel
    {
        public Guid? ProductId { get; set; }

        public  virtual Product? Product { get; set; }

        public string? FullName { get; set; }

        public string? Message { get; set; }
    }
}
