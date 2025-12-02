using FBS.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Orders
{
    public class OrderItemDto
    {
        public Guid? ProductId { get; set; }          
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? ProductSizeId { get; set; }
        public string? ProductSize { get; set; }

        public Guid? ProductColorId { get; set; }

        public string? ProductColor { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }
    }
}
