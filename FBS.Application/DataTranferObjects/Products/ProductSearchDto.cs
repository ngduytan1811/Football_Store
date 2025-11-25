using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Products
{
    public class ProductSearchDto
    {
        public Guid? CategoryId { get; set; }

        public string? SearchName { get; set; }
        public string? Sort { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }
        public List<string>? Sizes { get; set; } = new List<string>();
        public List<string>? Brands { get; set; } = new List<string>();

        public int Page { get; set; } = 1;
    }
}
