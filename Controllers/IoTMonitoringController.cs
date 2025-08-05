using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Models;
using RoomDeviceManagement.Services;
using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// IoT 监控系统 API 控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IoTMonitoringController : ControllerBase
    {
        private readonly IoTMonitoringService _iotService;
        private readonly ILogger<IoTMonitoringController> _logger;

        public IoTMonitoringController(IoTMonitoringService iotService, ILogger<IoTMonitoringController> logger)
        {
            _iotService = iotService;
            _logger = logger;
        }

        /// <summary>
        /// 智能设备状态监控 - 轮询所有设备状态
        /// </summary>
        [HttpGet("devices/poll-status")]
        public async Task<IActionResult> PollDeviceStatus()
        {
            try
            {
                var result = await _iotService.PollAllDeviceStatusAsync();
                return Ok(new { 
                    Success = true, 
                    Message = "设备状态轮询完成", 
                    Data = result,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备状态轮询失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 设备故障状态上报接口
        /// </summary>
        [HttpPost("devices/fault-report")]
        public async Task<IActionResult> ReportDeviceFault([FromBody] DeviceFaultReportDto faultReport)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _iotService.HandleDeviceFaultAsync(faultReport);
                return Ok(new { 
                    Success = true, 
                    Message = "设备故障上报处理成功", 
                    Data = result,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备故障上报处理失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 电子围栏 GPS 位置上报接口
        /// </summary>
        [HttpPost("fence/gps-report")]
        public async Task<IActionResult> ReportGpsLocation([FromBody] GpsLocationReportDto gpsReport)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _iotService.HandleGpsLocationAsync(gpsReport);
                
                // 检查 result 是否包含 IsOutOfFence 属性
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
        /// 健康监测数据上报接口
        /// </summary>
        [HttpPost("health/data-report")]
        public async Task<IActionResult> ReportHealthData([FromBody] HealthDataReportDto healthReport)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _iotService.HandleHealthDataAsync(healthReport);
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
        /// 获取实时监控仪表板数据
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetMonitoringDashboard()
        {
            try
            {
                var dashboard = await _iotService.GetMonitoringDashboardAsync();
                return Ok(new { 
                    Success = true, 
                    Data = dashboard,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取监控仪表板数据失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取异常警报列表
        /// </summary>
        [HttpGet("alerts")]
        public async Task<IActionResult> GetActiveAlerts()
        {
            try
            {
                var alerts = await _iotService.GetActiveAlertsAsync();
                return Ok(new { 
                    Success = true, 
                    Data = alerts,
                    Count = alerts.Count,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取警报列表失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取特定设备的详细状态信息
        /// </summary>
        [HttpGet("devices/{deviceId}/status")]
        public async Task<IActionResult> GetDeviceStatus(int deviceId)
        {
            try
            {
                var deviceStatus = await _iotService.GetDeviceStatusByIdAsync(deviceId);
                if (deviceStatus == null)
                {
                    return NotFound(new { Success = false, Message = $"设备 {deviceId} 不存在" });
                }

                return Ok(new { 
                    Success = true, 
                    Data = deviceStatus,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取设备 {deviceId} 状态失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取特定老人的健康监测历史数据
        /// </summary>
        [HttpGet("health/{elderlyId}/history")]
        public async Task<IActionResult> GetElderlyHealthHistory(int elderlyId, [FromQuery] int days = 7)
        {
            try
            {
                var healthHistory = await _iotService.GetElderlyHealthHistoryAsync(elderlyId, days);
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
        /// 获取电子围栏进出记录
        /// </summary>
        [HttpGet("fence/logs")]
        public async Task<IActionResult> GetFenceLogs([FromQuery] int? elderlyId = null, [FromQuery] int hours = 24)
        {
            try
            {
                var fenceLogs = await _iotService.GetFenceLogsAsync(elderlyId, hours);
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
        [HttpGet("fence/current-status")]
        public async Task<IActionResult> GetElderlyLocationStatus()
        {
            try
            {
                var locationStatus = await _iotService.GetElderlyLocationStatusAsync();
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
        /// 手动触发设备状态同步
        /// </summary>
        [HttpPost("devices/sync")]
        public async Task<IActionResult> SyncDeviceStatus()
        {
            try
            {
                var result = await _iotService.SyncAllDeviceStatusAsync();
                return Ok(new { 
                    Success = true, 
                    Message = "设备状态同步完成",
                    Data = result,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备状态同步失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }

    // DTO 类定义
    /// <summary>
    /// 设备故障上报数据传输对象
    /// </summary>
    public class DeviceFaultReportDto
    {
        [Required]
        public int DeviceId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string DeviceType { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string FaultStatus { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? FaultDescription { get; set; }
        
        public DateTime ReportTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// GPS位置上报数据传输对象
    /// </summary>
    public class GpsLocationReportDto
    {
        [Required]
        public int ElderlyId { get; set; }
        
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
        
        public DateTime LocationTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 健康数据上报数据传输对象
    /// </summary>
    public class HealthDataReportDto
    {
        [Required]
        public int ElderlyId { get; set; }
        
        public int? HeartRate { get; set; }
        
        [MaxLength(20)]
        public string? BloodPressure { get; set; }
        
        public float? OxygenLevel { get; set; }
        
        public float? Temperature { get; set; }
        
        public DateTime MeasurementTime { get; set; } = DateTime.Now;
    }
}
