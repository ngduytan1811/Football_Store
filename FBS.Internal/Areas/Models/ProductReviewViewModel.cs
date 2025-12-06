namespace FBS.Internal.Areas.Models
{
    public class ProductReviewViewModel
    {
        public Guid Id { get; set; }
        public string? ProductName { get; set; }
        public string? FullName { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
