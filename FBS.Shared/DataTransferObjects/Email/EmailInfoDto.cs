namespace FBS.Shared.DataTranferObjects.Email
{
    public class EmailInfoDto
    {
        public EmailInfoDto()
        {
            EmailAddress = new List<string>();
        }

        public string Title { get; set; }

        public List<string>? EmailAddress { get; set; }

        public string? Content { get; set; }

        public string? FileName { get; set; }

        public string? ResultTemplatePath { get; set; }

        public bool HasAttachment { get; set; }

        public byte[]? File { get; set; }

        public int CurrentUserId { get; set; }
    }
}
