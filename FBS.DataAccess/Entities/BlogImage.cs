using System.ComponentModel.DataAnnotations.Schema;

namespace FBS.Infrastructure.Entities
{
    public class BlogImage : BaseModel
    {
        public Guid BlogId { get; set; }
        public string Image { get; set; }

        [ForeignKey("BlogId")]
        public Blog Blog { get; set; }
    }
}
