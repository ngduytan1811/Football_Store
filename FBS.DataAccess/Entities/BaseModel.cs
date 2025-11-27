

namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using FBS.Shared.Enums;

    public class BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public StatusEnum? Status { get; set; } = StatusEnum.Active;

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; }

        public Guid? CreatedById { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedById { get; set; }
    }
}
