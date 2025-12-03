using FBS.Shared.DataTranferObjects.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Blog
{
    public class BlogSaveDto : BaseSaveDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ContentPart1 { get; set; }
        public string? ContentPart2 { get; set; }
        public string? Content { get; set; }

        // Ảnh chính
        public string? Thumbnail { get; set; }
        public IFormFile? ThumbnailFile { get; set; }

        // Ảnh phụ
        public List<IFormFile>? SubImageFiles { get; set; }
        public List<string>? SubImages { get; set; }
        public List<string> OldSubImages { get; set; } = new();

    }
}          


