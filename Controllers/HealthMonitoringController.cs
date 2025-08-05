using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Models;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// 健康监测 API 控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HealthMonitoringController : ControllerBase
    {
        private readonly HealthMonitoringService _healthService;
        private readonly ILogger<HealthMonitoringController> _logger;

        public HealthMonitoringController(HealthMonitoringService healthService, ILogger<HealthMonitoringController> logger)
        {
            _healthService = healthService;
            _logger = logger;
        }

        /// <summary>
        /// 健康监测数据上报接口
        /// </summary>
        [HttpPost("data-report")]
        public async Task<IActionResult> ReportHealthData([FromBody] HealthDataReportDto healthReport)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _healthService.HandleHealthDataAsync(healthReport);
                return Ok(new { 
                    Success = true, 
                    Message = "健康数据上报处理成功", 
                    Data = result,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "健康数据上报处理失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取特定老人的健康监测历史数据
        /// </summary>
        [HttpGet("elderly/{elderlyId}/history")]
        public async Task<IActionResult> GetElderlyHealthHistory(int elderlyId, [FromQuery] int days = 7)
        {
            try
            {
                var healthHistory = await _healthService.GetElderlyHealthHistoryAsync(elderlyId, days);
                return Ok(new { 
                    Success = true, 
                    Data = healthHistory,
                    ElderlyId = elderlyId,
                    Days = days,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取老人 {elderlyId} 健康历史数据失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取健康数据实时统计
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetHealthStatistics([FromQuery] int? elderlyId = null)
        {
            try
            {
                var statistics = await _healthService.GetHealthStatisticsAsync(elderlyId);
                return Ok(new { 
                    Success = true, 
                    Data = statistics,
                    ElderlyId = elderlyId,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取健康统计数据失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取健康异常警报
        /// </summary>
        [HttpGet("alerts")]
        public async Task<IActionResult> GetHealthAlerts([FromQuery] int? elderlyId = null, [FromQuery] bool activeOnly = true)
        {
            try
            {
                var alerts = await _healthService.GetHealthAlertsAsync(elderlyId, activeOnly);
                return Ok(new { 
                    Success = true, 
                    Data = alerts,
                    Count = alerts.Count,
                    ElderlyId = elderlyId,
                    ActiveOnly = activeOnly,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取健康警报失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取老人的最新健康数据
        /// </summary>
        [HttpGet("elderly/{elderlyId}/latest")]
        public async Task<IActionResult> GetLatestHealthData(int elderlyId)
        {
            try
            {
                var latestData = await _healthService.GetLatestHealthDataAsync(elderlyId);
                if (latestData == null)
                {
                    return NotFound(new { Success = false, Message = $"老人 {elderlyId} 暂无健康数据" });
                }

                return Ok(new { 
                    Success = true, 
                    Data = latestData,
                    ElderlyId = elderlyId,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取老人 {elderlyId} 最新健康数据失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取健康趋势分析
        /// </summary>
        [HttpGet("elderly/{elderlyId}/trends")]
        public async Task<IActionResult> GetHealthTrends(int elderlyId, [FromQuery] int days = 30)
        {
            try
            {
                var trends = await _healthService.GetHealthTrendsAsync(elderlyId, days);
                return Ok(new { 
                    Success = true, 
                    Data = trends,
                    ElderlyId = elderlyId,
                    Days = days,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取老人 {elderlyId} 健康趋势分析失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 批量上报健康数据
        /// </summary>
        [HttpPost("batch-report")]
        public async Task<IActionResult> BatchReportHealthData([FromBody] List<HealthDataReportDto> healthReports)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _healthService.HandleBatchHealthDataAsync(healthReports);
                return Ok(new { 
                    Success = true, 
                    Message = $"批量健康数据上报处理成功，共处理 {healthReports.Count} 条数据", 
                    Data = result,
                    Count = healthReports.Count,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量健康数据上报处理失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}
