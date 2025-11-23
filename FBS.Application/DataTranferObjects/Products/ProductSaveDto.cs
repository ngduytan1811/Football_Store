using FBS.Shared.DataTranferObjects.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Products
{
    public class ProductSaveDto : BaseSaveDto
    {
        public Guid? CategoryId { get; set; }

        [Required(ErrorMessage ="Tên sản phẩm là bắt buộc")]
        public string? Name { get; set; }
        public string? Color { get; set; }
        public List<string> Sizes { get; set; } = new();

        public string? Description { get; set; }
        public string? Detail { get; set; }
        public string? Image { get; set; }
        public decimal? Price { get; set; }
        public int? Discount { get; set; }
    }
}
