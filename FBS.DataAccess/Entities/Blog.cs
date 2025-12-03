using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Infrastructure.Entities
{
    public class Blog : BaseModel
    {
        public string Title { get; set; }
        public string Thumbnail { get; set; }   
        public string Content { get; set; }     
        public string Author { get; set; }
        public List<BlogImage> Images { get; set; } = new List<BlogImage>();

    }
}
