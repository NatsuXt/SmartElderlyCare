using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Models;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.DTOs;
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

        // GPS位置上报功能已移至 ElectronicFenceController

        // 健康数据上报功能已移至 HealthMonitoringController

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

        // 健康历史数据功能已移至 HealthMonitoringController

        // 围栏日志功能已移至 ElectronicFenceController

        // 老人位置状态功能已移至 ElectronicFenceController

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

}
