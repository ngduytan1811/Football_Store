using System.Text.Json.Serialization;

namespace FBS.Application.DataTranferObjects.VietQR
{
    public class VietQRResponse
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("desc")]
        public string? Description { get; set; }

        [JsonPropertyName("data")]
        public VietQRData? Data { get; set; }
    }
}
