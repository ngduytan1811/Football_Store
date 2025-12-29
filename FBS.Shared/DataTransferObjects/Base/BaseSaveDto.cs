namespace FBS.Shared.DataTranferObjects.Base
{
    using FBS.Shared.Enums;

    public class BaseSaveDto
    {
        public Guid? Id { get; set; }
        public StatusEnum? Status { get; set; } = StatusEnum.Active;
    }
}
