using FBS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Infrastructure.Entities
{
    public class OrderStatusHistory
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public StatusEnum Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Note { get; set; } 
    }

}
