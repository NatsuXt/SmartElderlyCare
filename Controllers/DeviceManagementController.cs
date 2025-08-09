using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceManagementController : ControllerBase
    {
        private readonly DeviceManagementService _deviceManagementService;
        private readonly ILogger<DeviceManagementController> _logger;

        public DeviceManagementController(DeviceManagementService deviceManagementService, ILogger<DeviceManagementController> logger)
        {
            _deviceManagementService = deviceManagementService;
            _logger = logger;
        }

        /// <summary>
        /// 获取设备列表（支持分页和搜索）
        /// </summary>
        /// <param name="page">页码，默认为1</param>
        /// <param name="pageSize">每页条数，默认为20</param>
        /// <param name="search">搜索关键词（设备名称或设备类型）</param>
        /// <param name="sortBy">排序字段（deviceName, deviceType, status, installationDate）</param>
        /// <param name="sortDesc">是否降序排列</param>
        [HttpGet("devices")]
        public async Task<ActionResult<ApiResponse<List<DeviceDetailDto>>>> GetDevices(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false)
        {
            try
            {
                // 参数验证
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var request = new PagedRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    Search = search,
                    SortBy = sortBy,
                    SortDesc = sortDesc
                };

                var result = await _deviceManagementService.GetDevicesAsync(request);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取设备列表失败");
                return StatusCode(500, new ApiResponse<List<DeviceDetailDto>>
                {
                    Success = false,
                    Message = "获取设备列表失败：" + ex.Message
                });
            }
        }

        /// <summary>
        /// 根据ID获取设备详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DeviceDetailDto>>> GetDevice(int id)
        {
            var result = await _deviceManagementService.GetDeviceByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// 创建新设备
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<DeviceDetailDto>>> CreateDevice([FromBody] DeviceCreateDto dto)
        {
            var result = await _deviceManagementService.CreateDeviceAsync(dto);
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetDevice), new { id = result.Data?.DeviceId }, result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// 更新设备信息
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<DeviceDetailDto>>> UpdateDevice(int id, [FromBody] DeviceUpdateDto dto)
        {
            var result = await _deviceManagementService.UpdateDeviceAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteDevice(int id)
        {
            var result = await _deviceManagementService.DeleteDeviceAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 获取设备统计信息
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetDeviceStatistics()
        {
            try
            {
                var devicesResult = await _deviceManagementService.GetDevicesAsync(new PagedRequest { Page = 1, PageSize = 1000 });
                if (!devicesResult.Success)
                {
                    return BadRequest(devicesResult);
                }

                var devices = devicesResult.Data ?? new List<DeviceDetailDto>();
                var stats = new
                {
                    总设备数 = devices.Count,
                    正常设备 = devices.Count(d => d.Status == "正常"),
                    故障设备 = devices.Count(d => d.Status == "故障"),
                    维护中设备 = devices.Count(d => d.Status == "维护中"),
                    设备类型分布 = devices.GroupBy(d => d.DeviceType)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    状态分布 = devices.GroupBy(d => d.Status)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    已分配房间设备 = devices.Count(d => d.RoomId.HasValue),
                    未分配房间设备 = devices.Count(d => !d.RoomId.HasValue)
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "获取设备统计信息成功",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取设备统计信息失败");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"获取设备统计信息失败: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 智能设备状态监控 - 轮询所有设备状态（从IoT模块迁移）
        /// </summary>
        [HttpGet("poll-status")]
        public async Task<IActionResult> PollDeviceStatus()
        {
            try
            {
                var result = await _deviceManagementService.PollAllDeviceStatusAsync();
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备状态轮询失败");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"设备状态轮询失败: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 设备故障状态上报接口（从IoT模块迁移）
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

                var result = await _deviceManagementService.HandleDeviceFaultAsync(faultReport);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备故障上报处理失败");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"设备故障上报处理失败: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 手动触发设备状态同步（从IoT模块迁移）
        /// </summary>
        [HttpPost("sync")]
        public async Task<IActionResult> SyncDeviceStatus()
        {
            try
            {
                var result = await _deviceManagementService.SyncAllDeviceStatusAsync();
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备状态同步失败");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"设备状态同步失败: {ex.Message}"
                });
            }
        }
    }
}
