

namespace FBS.Infrastructure.Entities
{
    using FBS.Shared.Enums;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Orders")]
    public class Order : BaseModel
    {
        public Guid? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerEmail { get; set; }

        public string? CustomerAddress { get; set; }

        public string? Note { get; set; }
        public string PaymentMethod { get; set; }           
        public PaymentStatusEnum PaymentStatus { get; set; } 
        public StatusEnum Status { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
