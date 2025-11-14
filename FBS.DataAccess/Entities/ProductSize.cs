using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Infrastructure.Entities
{
    [Table("ProductSizes")]
    public class ProductSize : BaseModel
    {
        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; }

        public string? Size { get; set; }
    }
}
