using System.Text.Json.Serialization;

namespace FBS.Application.DataTranferObjects.VietQR
{
    public class VietQRData
    {
        [JsonPropertyName("qrDataURL")]
        public string? QrDataUrl { get; set; }
    }
}