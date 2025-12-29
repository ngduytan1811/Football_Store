

namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OrderItems")]
    public class OrderItem : BaseModel
    {
        public Guid? OrderId { get; set; }

        public virtual Order? Order { get; set; }

        public string? ProductSize { get; set; }

        public string? ProductColor { get; set; }

        public Guid? ProductId { get; set; }

        public virtual Product? Product { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }
    }
}
