using FBS.Shared.DataTranferObjects.Base;
using Microsoft.AspNetCore.Http;
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
        
        public decimal? Price { get; set; }
        public string? PriceString => Price.HasValue ? Price.Value.ToString("C", new CultureInfo("vi-VN")) : string.Empty;
        public List<string> Sizes { get; set; } = new();
        public string? Color { get; set; }
        public string? Brand { get; set; }
        public int? Discount { get; set; }

        public List<ProductReivewDto> Reviews { get; set; } = new List<ProductReivewDto>();
    }

    public class ProductReivewDto
    {
        public string? FullName { get; set; }
        public string? Message { get; set; }
    }
}
