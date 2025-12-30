using System;
using System.Collections.Generic;
using System.Globalization;

namespace FBS.Application.DataTranferObjects.Cart
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;

        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

 
        public decimal? Price { get; set; }

        public int Quantity { get; set; }

        
        public decimal SubPrice => (Price ?? 0m) * Quantity;

       
        public string PriceString =>
            (Price ?? 0m).ToString("N0", new CultureInfo("vi-VN")) + " đ";

        public string SubPriceString =>
            SubPrice.ToString("N0", new CultureInfo("vi-VN")) + " đ";

     
        public string Description { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public List<string> SubImages { get; set; } = new();
    }
}
