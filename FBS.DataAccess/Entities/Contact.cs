namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Contacts")]
    public class Contact : BaseModel
    {
        public Guid? UserId { get; set; }

        public virtual User? User { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Message { get; set; }

        public bool IsDeleted { get; set; }
    }
}
