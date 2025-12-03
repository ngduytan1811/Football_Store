using FBS.Shared.DataTranferObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Application.DataTranferObjects.Blog
{
    public class BlogDto: BaseDto
    {
        public string? Title { get; set; }
        public string? Thumbnail { get; set; }
        public string? Content { get; set; }
        public string? Author { get; set; }
        public List<string> Images { get; set; }
    }
}
