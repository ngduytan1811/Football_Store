namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Categories")]
    public class Category : BaseModel
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Logo { get; set; }

        public Guid? ParentId { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
