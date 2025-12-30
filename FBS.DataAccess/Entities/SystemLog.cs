namespace FBS.Infrastructure.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using FBS.Shared.Enums;

    [Table("SystemLogs")]
    public class SystemLog : BaseModel
    {
        public string? FieldName { get; set; }

        public string? Screen { get; set; }

        public string? NewValue { get; set; }

        public string? OldValue { get; set; }

        public SystemLogActionEnum? Action { get; set; }

        public Guid? ObjectId { get; set; }

        public string? ObjectName { get; set; }
    }
}
