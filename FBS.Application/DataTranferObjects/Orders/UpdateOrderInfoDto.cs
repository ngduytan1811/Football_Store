using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Orders
{
    public class UpdateOrderInfoDto
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string? Note { get; set; }
    }
}
