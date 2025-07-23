using System.Text;
using ElderlyCareSystem.Data;
using ElderlyCareSystem.Dtos;
using ElderlyCareSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ElderlyCareSystem.Services
{
    public class DietRecommendationService
    {
        private readonly AppDbContext _context;
        private readonly QianfanService _qianfanService;

        public DietRecommendationService(AppDbContext context, QianfanService qianfanService)
        {
            _context = context;
            _qianfanService = qianfanService;
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
你是一名营养专家，请根据以下健康指标简要推荐适合老人的10种食物（只列出食物名称，用顿号分隔）：
- 心率: {monitoring.HeartRate} 次/分
- 血压: {monitoring.BloodPressure}
- 血氧: {monitoring.OxygenLevel}%
- 体温: {monitoring.Temperature}℃
- 身体机能评分: {report.PhysicalHealthFunction}
- 心理功能评分: {report.PsychologicalFunction}
- 认知功能评分: {report.CognitiveFunction}
- 健康等级: {report.HealthGrade}
""";

            try
            {
                string result = await _qianfanService.GetChatCompletionAsync(prompt);

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
                return $"生成失败: {ex.Message}";
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

        public async Task<List<DietRecommendationDto>> GetRecommendationsByElderlyAsync(int elderlyId)
        {
            return await _context.DietRecommendations
                .Where(r => r.ElderlyId == elderlyId)
                .OrderByDescending(r => r.RecommendationDate)
                .Select(r => new DietRecommendationDto
                {
                    RecommendationId = r.RecommendationId,
                    ElderlyId = r.ElderlyId,
                    RecommendationDate = r.RecommendationDate,
                    RecommendedFood = r.RecommendedFood,
                    ExecutionStatus = r.ExecutionStatus
                })
                .ToListAsync();
        }
    }
}
