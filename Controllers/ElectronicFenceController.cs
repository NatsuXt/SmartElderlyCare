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

        /// <summary>
        /// 创建或更新围栏配置
        /// </summary>
        [HttpPost("config")]
        public async Task<IActionResult> CreateOrUpdateFenceConfig([FromBody] FenceConfigDto fenceConfig)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _fenceService.CreateOrUpdateFenceConfigAsync(fenceConfig);
                return Ok(new { 
                    Success = true, 
                    Message = "围栏配置保存成功", 
                    Data = result,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存围栏配置失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 删除围栏配置
        /// </summary>
        [HttpDelete("config/{fenceId}")]
        public async Task<IActionResult> DeleteFenceConfig(int fenceId)
        {
            try
            {
                var result = await _fenceService.DeleteFenceConfigAsync(fenceId);
                return Ok(new { 
                    Success = true, 
                    Message = "围栏配置删除成功", 
                    Data = result,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除围栏配置失败: FenceId {fenceId}");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取护理人员位置信息
        /// </summary>
        [HttpGet("staff-locations")]
        public async Task<IActionResult> GetStaffLocations()
        {
            try
            {
                var staffLocations = await _fenceService.GetStaffLocationsAsync();
                return Ok(new { 
                    Success = true, 
                    Data = staffLocations,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取护理人员位置失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 更新护理人员位置
        /// </summary>
        [HttpPost("staff-location")]
        public async Task<IActionResult> UpdateStaffLocation([FromBody] StaffLocationUpdateDto locationUpdate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _fenceService.UpdateStaffLocationAsync(locationUpdate);
                return Ok(new { 
                    Success = true, 
                    Message = "护理人员位置更新成功", 
                    Data = result,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新护理人员位置失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 测试围栏检查功能
        /// </summary>
        [HttpPost("test-fence")]
        public async Task<IActionResult> TestFenceCheck([FromBody] FenceTestDto testData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _fenceService.TestFenceCheckAsync(testData.Latitude, testData.Longitude);
                return Ok(new { 
                    Success = true, 
                    Message = "围栏检查测试完成", 
                    Data = result,
                    TestLocation = new { testData.Latitude, testData.Longitude },
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "围栏检查测试失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}
