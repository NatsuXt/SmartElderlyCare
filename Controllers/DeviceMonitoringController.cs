using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Models;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// 智能设备状态监控 API 控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceMonitoringController : ControllerBase
    {
        private readonly DeviceMonitoringService _deviceService;
        private readonly ILogger<DeviceMonitoringController> _logger;

        public DeviceMonitoringController(DeviceMonitoringService deviceService, ILogger<DeviceMonitoringController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        /// <summary>
        /// 轮询所有设备状态
        /// </summary>
        [HttpGet("poll-status")]
        public async Task<IActionResult> PollDeviceStatus()
        {
            try
            {
                var result = await _deviceService.PollAllDeviceStatusAsync();
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
        [HttpPost("fault-report")]
        public async Task<IActionResult> ReportDeviceFault([FromBody] DeviceFaultReportDto faultReport)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _deviceService.HandleDeviceFaultAsync(faultReport);
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
        /// 获取特定设备的详细状态信息
        /// </summary>
        [HttpGet("{deviceId}/status")]
        public async Task<IActionResult> GetDeviceStatus(int deviceId)
        {
            try
            {
                var deviceStatus = await _deviceService.GetDeviceStatusByIdAsync(deviceId);
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
        /// 手动触发设备状态同步
        /// </summary>
        [HttpPost("sync")]
        public async Task<IActionResult> SyncDeviceStatus()
        {
            try
            {
                var result = await _deviceService.SyncAllDeviceStatusAsync();
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

        /// <summary>
        /// 获取设备故障历史记录
        /// </summary>
        [HttpGet("fault-history")]
        public async Task<IActionResult> GetDeviceFaultHistory([FromQuery] int? deviceId = null, [FromQuery] int days = 30)
        {
            try
            {
                var faultHistory = await _deviceService.GetDeviceFaultHistoryAsync(deviceId, days);
                return Ok(new { 
                    Success = true, 
                    Data = faultHistory,
                    DeviceId = deviceId,
                    Days = days,
                    Timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取设备故障历史失败");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}
