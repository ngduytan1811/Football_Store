using FBS.Shared.DataTranferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Orders
{
    public class OrderDto : BaseDto
    {
        public Guid? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public string? CustomerEmail { get; set; }

        public string? CustomerAddress { get; set; }

        public string? Note { get; set; }


        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
