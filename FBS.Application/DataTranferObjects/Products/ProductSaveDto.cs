using FBS.Shared.DataTranferObjects.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBS.Application.DataTranferObjects.Products
{
    public class ProductSaveDto : BaseSaveDto
    {
        public Guid? CategoryId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        public string? Name { get; set; }

        public string? Color { get; set; }

        public List<string> Sizes { get; set; } = new();

        public string? Description { get; set; }

        public string? Detail { get; set; }
        public string? DetailPart1 { get; set; }
        public string? DetailPart2 { get; set; }

     
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
 

        
        public List<IFormFile>? SubImageFiles { get; set; }

       
        public List<string>? SubImages { get; set; }

    
        public List<string>? OldSubImages { get; set; }

        public string? Brand { get; set; }

        public decimal? Price { get; set; }

        public int? Discount { get; set; }
    }
}
