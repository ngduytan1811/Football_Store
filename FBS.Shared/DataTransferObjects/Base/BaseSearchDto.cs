namespace FBS.Shared.DataTranferObjects.Base
{
    using FBS.Shared.Constants;

    public class BaseSearchDto<T>
        where T : class
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = GlobalConstants.DefaultPageSize;

        public string ColumnSort { get; set; } = ColumnNames.CreatedAt;

        public bool Asc { get; set; } = false;

        public T? SearchParams { get; set; }

        public int Start
        {
            get
            {
                return Page == 0 ? 0 : (Page - 1) * PageSize;
            }
        }
    }
}
