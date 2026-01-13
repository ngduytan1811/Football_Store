using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Infrastructure.Entities
{
    [Table("ProductColors")]
    public class ProductColor : BaseModel
    {
        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; }

        public string Color { get; set; } = null!;

        public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

    }
}
