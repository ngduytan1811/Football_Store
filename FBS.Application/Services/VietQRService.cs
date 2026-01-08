using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FBS.Application.DataTranferObjects.VietQR;
using FBS.Shared.Enums;

namespace FBS.Application.Services
{
    public class VietQRService
    {
        private readonly HttpClient _httpClient;

        public VietQRService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateVietQRAsync(
            string accountNo,
            string accountName,
            BankEnum bank,
            decimal amount,
            string note
        )
        {
            var request = new VietQRRequest
            {
                acqId = (int)(bank),
                accountNo = accountNo,
                accountName = accountName,
                amount = (int)amount,
                addInfo = note,
                format = "text",
                template = "compact"
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "https://api.vietqr.io/v2/generate",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"VietQR API error: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<VietQRResponse>(responseJson);

            if (result?.Code != "00" || result.Data?.QrDataUrl == null)
            {
                throw new Exception(
                    $"VietQR failed: {result?.Description ?? "Unknown error"}");
            }

            return result.Data.QrDataUrl;
        }

       
    }
}
