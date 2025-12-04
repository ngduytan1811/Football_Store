using FBS.Shared.DataTranferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Reports
{
    public class RevenueChartDto : BaseSaveDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}
