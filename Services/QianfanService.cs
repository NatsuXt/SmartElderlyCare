using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ElderlyCareSystem.Services
{
    public class QianfanService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _accessToken;
        private readonly string _apiUrl;

        public QianfanService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _accessToken = configuration["Qianfan:AccessToken"];
            _apiUrl = configuration["Qianfan:ApiUrl"] ?? "https://qianfan.baidubce.com/v2/chat/completions";
        }

        public async Task<string> GetChatCompletionAsync(string prompt, string model = "ernie-4.0-turbo-8k", double temperature = 0.8)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestBody = new
            {
                messages = new[] { new { role = "user", content = prompt } },
                model,
                temperature
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(_apiUrl, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"千帆接口调用失败: {response.StatusCode} - {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseContent);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }
    }
}
