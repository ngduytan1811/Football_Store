using FBS.Shared.DataTranferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Reports
{
    public class TopProductDto: BaseSaveDto
    {
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int TotalQuantity { get; set; }
    }
}
