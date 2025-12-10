using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Cart
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Image { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }

        public string? PriceString => Price.HasValue ? Price.Value.ToString("C", new CultureInfo("vi-VN")) : string.Empty;
        public int Quantity { get; set; }
        public decimal SubPrice => (Price ?? 0) * Quantity;
        public string? SubPriceString => SubPrice.ToString("C", new CultureInfo("vi-VN"));
    }
}
