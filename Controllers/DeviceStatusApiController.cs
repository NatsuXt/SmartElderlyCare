using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.Models;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// 设备状态API控制器
    /// </summary>
    [ApiController]
    [Route("api/devices")]
    [Produces("application/json")]
    public class DeviceStatusApiController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public DeviceStatusApiController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// 获取所有设备
        /// </summary>
        /// <returns>设备列表</returns>
        [HttpGet]
        public ActionResult<List<DeviceStatus>> GetAllDevices()
        {
            try
            {
                var devices = _databaseService.GetAllDevices();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取设备列表失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取设备
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>设备信息</returns>
        [HttpGet("{deviceId:int}")]
        public ActionResult<DeviceStatus> GetDeviceById(int deviceId)
        {
            try
            {
                var device = _deviceService.GetDeviceById(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"未找到ID为 {deviceId} 的设备" });
                }
                return Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取设备信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据房间号获取设备
        /// </summary>
        /// <param name="roomNumber">房间号</param>
        /// <returns>设备列表</returns>
        [HttpGet("room/{roomNumber}")]
        public ActionResult<List<DeviceStatus>> GetDevicesByRoom(string roomNumber)
        {
            try
            {
                var devices = _deviceService.GetDevicesByRoom(roomNumber);
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取房间设备失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据设备类型获取设备
        /// </summary>
        /// <param name="deviceType">设备类型</param>
        /// <returns>设备列表</returns>
        [HttpGet("type/{deviceType}")]
        public ActionResult<List<DeviceStatus>> GetDevicesByType(string deviceType)
        {
            try
            {
                var devices = _deviceService.GetDevicesByType(deviceType);
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取设备类型失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据状态获取设备
        /// </summary>
        /// <param name="status">设备状态</param>
        /// <returns>设备列表</returns>
        [HttpGet("status/{status}")]
        public ActionResult<List<DeviceStatus>> GetDevicesByStatus(string status)
        {
            try
            {
                var devices = _deviceService.GetDevicesByStatus(status);
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取设备状态失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="device">设备信息</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public ActionResult AddDevice([FromBody] DeviceStatus device)
        {
            try
            {
                if (device == null)
                {
                    return BadRequest(new { message = "设备信息不能为空" });
                }

                var result = _deviceService.AddDevice(device);
                if (result)
                {
                    return Ok(new { message = "设备添加成功", deviceId = device.DeviceId });
                }
                else
                {
                    return BadRequest(new { message = "设备添加失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "添加设备失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 更新设备
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="device">设备信息</param>
        /// <returns>操作结果</returns>
        [HttpPut("{deviceId:int}")]
        public ActionResult UpdateDevice(int deviceId, [FromBody] DeviceStatus device)
        {
            try
            {
                if (device == null)
                {
                    return BadRequest(new { message = "设备信息不能为空" });
                }

                device.DeviceId = deviceId;
                var result = _deviceService.UpdateDevice(device);
                if (result)
                {
                    return Ok(new { message = "设备更新成功" });
                }
                else
                {
                    return NotFound(new { message = "设备不存在或更新失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新设备失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{deviceId:int}")]
        public ActionResult DeleteDevice(int deviceId)
        {
            try
            {
                var result = _deviceService.DeleteDevice(deviceId);
                if (result)
                {
                    return Ok(new { message = "设备删除成功" });
                }
                else
                {
                    return NotFound(new { message = "设备不存在或删除失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "删除设备失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取故障设备
        /// </summary>
        /// <returns>故障设备列表</returns>
        [HttpGet("faults")]
        public ActionResult<List<DeviceStatus>> GetFaultyDevices()
        {
            try
            {
                var faultyDevices = _deviceService.GetFaultyDevices();
                return Ok(faultyDevices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取故障设备失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取设备统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        [HttpGet("statistics")]
        public ActionResult GetDeviceStatistics()
        {
            try
            {
                var statistics = _deviceService.GetDeviceStatistics();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取设备统计失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 批量更新设备状态
        /// </summary>
        /// <param name="deviceIds">设备ID列表</param>
        /// <param name="status">新状态</param>
        /// <returns>操作结果</returns>
        [HttpPut("batch-status")]
        public ActionResult BatchUpdateDeviceStatus([FromBody] List<int> deviceIds, [FromQuery] string status)
        {
            try
            {
                if (deviceIds == null || !deviceIds.Any())
                {
                    return BadRequest(new { message = "设备ID列表不能为空" });
                }

                var result = _deviceService.BatchUpdateDeviceStatus(deviceIds, status);
                return Ok(new { message = $"批量更新成功，共更新 {result} 台设备" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "批量更新设备状态失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 搜索设备
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>搜索结果</returns>
        [HttpGet("search")]
        public ActionResult<List<DeviceStatus>> SearchDevices([FromQuery] string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new { message = "搜索关键词不能为空" });
                }

                var devices = _deviceService.SearchDevices(keyword);
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "搜索设备失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 重启设备
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{deviceId:int}/restart")]
        public ActionResult RestartDevice(int deviceId)
        {
            try
            {
                var result = _deviceService.RestartDevice(deviceId);
                if (result)
                {
                    return Ok(new { message = "设备重启成功" });
                }
                else
                {
                    return NotFound(new { message = "设备不存在或重启失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "重启设备失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 设备故障检测
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>检测结果</returns>
        [HttpPost("{deviceId:int}/fault-detection")]
        public ActionResult FaultDetection(int deviceId)
        {
            try
            {
                var result = _deviceService.FaultDetection(deviceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "故障检测失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取设备维护记录
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>维护记录</returns>
        [HttpGet("{deviceId:int}/maintenance")]
        public ActionResult GetMaintenanceRecords(int deviceId)
        {
            try
            {
                var records = _deviceService.GetMaintenanceRecords(deviceId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取维护记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 添加维护记录
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="maintenanceInfo">维护信息</param>
        /// <returns>操作结果</returns>
        [HttpPost("{deviceId:int}/maintenance")]
        public ActionResult AddMaintenanceRecord(int deviceId, [FromBody] dynamic maintenanceInfo)
        {
            try
            {
                var result = _deviceService.AddMaintenanceRecord(deviceId, maintenanceInfo);
                if (result)
                {
                    return Ok(new { message = "维护记录添加成功" });
                }
                else
                {
                    return BadRequest(new { message = "维护记录添加失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "添加维护记录失败", error = ex.Message });
            }
        }
    }
}
