namespace FBS.Internal.Areas.Models
{
    public class ProductSizeViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductColorId { get; set; }
        public string Size { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
