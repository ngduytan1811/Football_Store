using FBS.Shared.DataTranferObjects.Base;
using System.Globalization;

namespace FBS.Application.DataTranferObjects.Products
{
    public class ProductDto : BaseDto
    {
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        public string? Image { get; set; }

        
        public List<string> SubImages { get; set; } = new();

        public decimal? Price { get; set; }
        public string? PriceString =>
            Price.HasValue ? Price.Value.ToString("C", new CultureInfo("vi-VN")) : string.Empty;

        public List<string> Sizes { get; set; } = new();
        public string? Color { get; set; }
        public string? Brand { get; set; }
        public int? Discount { get; set; }

   

        public string? PriceAfterDiscountString
        {
            get
            {
                if (!Price.HasValue) return string.Empty;
                var finalPrice = Price.Value;
                if (Discount.HasValue && Discount.Value > 0)
                {
                    finalPrice -= finalPrice * Discount.Value / 100m;
                }
                return finalPrice.ToString("C", new CultureInfo("vi-VN"));
            }
        }
        public List<ProductReivewDto> Reviews { get; set; } = new List<ProductReivewDto>();
    }

    public class ProductReivewDto
    {
        public string? FullName { get; set; }
        public string? Message { get; set; }
    }
}
