using FBS.Shared.DataTranferObjects.Base;
using Microsoft.AspNetCore.Http;

namespace FBS.Application.DataTranferObjects.Categories
{
    public class CategorySaveDto : BaseSaveDto
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public IFormFile? Logo { get; set; }

        public Guid? ParentId { get; set; }
    }
}
