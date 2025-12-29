namespace FBS.Shared.DataTranferObjects.Base
{
    using FBS.Shared.Enums;
    using FBS.Shared.Helpers;

    public class BaseDto
    {
        public int Index { get; set; }

        public Guid Id { get; set; }

        public StatusEnum? Status { get; set; }

        public string? StatusName => Status.HasValue ? EnumHelper<StatusEnum>.GetDisplayValue((int)Status) : null;

        public DateTime? CreatedAt { get; set; }

        public Guid? CreatedById { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid? UpdatedById { get; set; }
    }
}
