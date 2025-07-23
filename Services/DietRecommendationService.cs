using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ElderlyCareSystem.Data;
using ElderlyCareSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ElderlyCareSystem.Services
{
    public class DietRecommendationService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string accessToken = "bce-v3/ALTAK-3DxPuNUfiBjvfFhPtK1yt/598449108e33d66de3d77b7c34b8b6791fca3544"; // 替换为你的真实 Token
        private readonly string qianfanApiUrl = "https://qianfan.baidubce.com/v2/chat/completions";

        public DietRecommendationService(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GenerateDietAdviceAsync(int elderlyId)
        {
            var monitoring = await _context.HealthMonitorings
                .Where(h => h.ElderlyId == elderlyId)
                .OrderByDescending(h => h.MonitoringDate)
                .FirstOrDefaultAsync();

            var report = await _context.HealthAssessmentReports
                .Where(r => r.ElderlyId == elderlyId)
                .OrderByDescending(r => r.AssessmentDate)
                .FirstOrDefaultAsync();

            if (monitoring == null || report == null)
                return "缺少健康数据，无法生成建议";

            string prompt = $"""
你是一名营养专家，请根据以下健康指标生成适合老人的个性化饮食建议：
- 心率: {monitoring.HeartRate} 次/分
- 血压: {monitoring.BloodPressure}
- 血氧: {monitoring.OxygenLevel}%
- 体温: {monitoring.Temperature}℃
- 身体机能评分: {report.PhysicalHealthFunction}
- 心理功能评分: {report.PsychologicalFunction}
- 认知功能评分: {report.CognitiveFunction}
- 健康等级: {report.HealthGrade}
简要推荐适合该老人的食物（仅列出食物名称，用顿号分隔，最多列10个）
""";

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                model = "ernie-4.0-turbo-8k",
                temperature = 0.8
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(qianfanApiUrl, content);

            if (!response.IsSuccessStatusCode)
                return $"生成失败: {response.StatusCode} - {response.ReasonPhrase}";

            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(responseContent);
                string result = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                var recommendation = new DietRecommendation
                {
                    ElderlyId = elderlyId,
                    RecommendationDate = DateTime.Now,
                    RecommendedFood = result,
                    ExecutionStatus = "待执行"
                };

                _context.DietRecommendations.Add(recommendation);
                await _context.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                return $"解析失败: {ex.Message}\n原始返回: {responseContent}";
            }
        }

        public async Task<bool> UpdateExecutionStatusAsync(int recommendationId, string status)
        {
            var recommendation = await _context.DietRecommendations.FindAsync(recommendationId);
            if (recommendation == null) return false;

            recommendation.ExecutionStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<DietRecommendation>> GetRecommendationsByElderlyAsync(int elderlyId)
        {
            return await _context.DietRecommendations
                .Where(r => r.ElderlyId == elderlyId)
                .OrderByDescending(r => r.RecommendationDate)
                .ToListAsync();
        }
    }
}
