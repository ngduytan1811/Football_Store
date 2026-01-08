using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.VietQR
{
    public class VietQRRequest
    {
        public int acqId { get; set; }         
        public string accountNo { get; set; }   
        public string accountName { get; set; } 
        public int amount { get; set; }         
        public string addInfo { get; set; }     
        public string format { get; set; } = "text";
        public string template { get; set; } = "compact";
    }

}
