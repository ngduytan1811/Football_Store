using FBS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Orders
{
    public class OrderStatusHistoryDto
    {
        public StatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Note { get; set; }
    }
}
