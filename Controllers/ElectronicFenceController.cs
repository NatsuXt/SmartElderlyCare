using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Models;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// 电子围栏监控 API 控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ElectronicFenceController : ControllerBase
    {
        private readonly ElectronicFenceService _fenceService;
        private readonly ILogger<ElectronicFenceController> _logger;

        public ElectronicFenceController(ElectronicFenceService fenceService, ILogger<ElectronicFenceController> logger)
        {
            _fenceService = fenceService;
            _logger = logger;
        }

        /// <summary>
        /// GPS 位置上报接口
        /// </summary>
        [HttpPost("gps-report")]
        public async Task<IActionResult> ReportGpsLocation([FromBody] GpsLocationReportDto gpsReport)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _fenceService.HandleGpsLocationAsync(gpsReport);
                
                // 检查是否越界
                bool isOutOfFence = false;
                if (result is { } resultObj)
                {
                    var props = resultObj.GetType().GetProperties();
                    var prop = props.FirstOrDefault(p => p.Name == "IsOutOfFence");
                    if (prop != null)
                    {
                        isOutOfFence = (bool)(prop.GetValue(resultObj) ?? false);
                    }
                }
                
                return Ok(new { 
                    Success = true, 
                    Message = "GPS位置上报处理成功", 
                    Data = result,
                    IsOutOfFence = isOutOfFence,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GPS位置上报处理失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取电子围栏进出记录
        /// </summary>
        [HttpGet("logs")]
        public async Task<IActionResult> GetFenceLogs([FromQuery] int? elderlyId = null, [FromQuery] int hours = 24)
        {
            try
            {
                var fenceLogs = await _fenceService.GetFenceLogsAsync(elderlyId, hours);
                return Ok(new { 
                    Success = true, 
                    Data = fenceLogs,
                    ElderlyId = elderlyId,
                    Hours = hours,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取围栏日志失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取所有老人的当前位置状态
        /// </summary>
        [HttpGet("current-status")]
        public async Task<IActionResult> GetElderlyLocationStatus()
        {
            try
            {
                var locationStatus = await _fenceService.GetElderlyLocationStatusAsync();
                return Ok(new { 
                    Success = true, 
                    Data = locationStatus,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取老人位置状态失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取围栏配置信息
        /// </summary>
        [HttpGet("config")]
        public async Task<IActionResult> GetFenceConfig()
        {
            try
            {
                var fenceConfig = await _fenceService.GetFenceConfigAsync();
                return Ok(new { 
                    Success = true, 
                    Data = fenceConfig,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取围栏配置失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取特定老人的位置轨迹
        /// </summary>
        [HttpGet("elderly/{elderlyId}/trajectory")]
        public async Task<IActionResult> GetElderlyTrajectory(int elderlyId, [FromQuery] int hours = 24)
        {
            try
            {
                var trajectory = await _fenceService.GetElderlyTrajectoryAsync(elderlyId, hours);
                return Ok(new { 
                    Success = true, 
                    Data = trajectory,
                    ElderlyId = elderlyId,
                    Hours = hours,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取老人 {elderlyId} 位置轨迹失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取围栏异常警报
        /// </summary>
        [HttpGet("alerts")]
        public async Task<IActionResult> GetFenceAlerts([FromQuery] bool activeOnly = true)
        {
            try
            {
                var alerts = await _fenceService.GetFenceAlertsAsync(activeOnly);
                return Ok(new { 
                    Success = true, 
                    Data = alerts,
                    Count = alerts.Count,
                    ActiveOnly = activeOnly,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取围栏警报失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}
