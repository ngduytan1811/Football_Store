using FBS.Shared.DataTranferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Categories
{
    public class CategoryDto : BaseDto
    {
        public string? Code { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Logo { get; set; }

        public int? Order { get; set; }

        public Guid? ParentId { get; set; }
        public string? ParentName { get; set; }
    }
}
