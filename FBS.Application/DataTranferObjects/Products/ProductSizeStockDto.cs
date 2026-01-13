using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Products
{
    public class ProductSizeStockDto
    {
        public Guid Id { get; set; }
        public string Size { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
