using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Infrastructure.Entities
{
    [Table("ProductSizes")]
    public class ProductSize : BaseModel
    {
        public Guid ProductColorId { get; set; }
        public virtual ProductColor ProductColor { get; set; }  
      
        [MaxLength(15)]
        public string? Size { get; set; } = null!;

        public int Quantity {  get; set; }
    }
}
