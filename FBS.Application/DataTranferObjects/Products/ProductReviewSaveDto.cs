using FBS.Shared.DataTranferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Products
{
    public class ProductReviewSaveDto : BaseSaveDto
    {
        public Guid ProductId { get; set; }

        public string? FullName { get; set; }

        public string? Message { get; set; }
    }
}
