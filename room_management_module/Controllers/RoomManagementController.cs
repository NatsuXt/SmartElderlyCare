using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomManagementController : ControllerBase
    {
        private readonly RoomManagementService _roomManagementService;
        private readonly ILogger<RoomManagementController> _logger;

        public RoomManagementController(RoomManagementService roomManagementService, ILogger<RoomManagementController> logger)
        {
            _roomManagementService = roomManagementService;
            _logger = logger;
        }

        /// <summary>
        /// 获取房间列表（支持分页和搜索）
        /// </summary>
        /// <param name="page">页码，默认为1</param>
        /// <param name="pageSize">每页条数，默认为20</param>
        /// <param name="search">搜索关键词（房间号或房间类型）</param>
        /// <param name="sortBy">排序字段（roomNumber, roomType, capacity, floor）</param>
        /// <param name="sortDesc">是否降序排列</param>
        [HttpGet("rooms")]
        public async Task<ActionResult<ApiResponse<List<RoomDetailDto>>>> GetRooms(
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

                var result = await _roomManagementService.GetRoomsAsync(request);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取房间列表失败");
                return StatusCode(500, new ApiResponse<List<RoomDetailDto>>
                {
                    Success = false,
                    Message = "服务器内部错误"
                });
            }
        }

        /// <summary>
        /// 根据ID获取房间详情
        /// </summary>
        /// <param name="id">房间ID</param>
        [HttpGet("rooms/{id}")]
        public async Task<ActionResult<ApiResponse<RoomDetailDto>>> GetRoom(int id)
        {
            try
            {
                var result = await _roomManagementService.GetRoomByIdAsync(id);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取房间详情失败，房间ID: {RoomId}", id);
                return StatusCode(500, new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = "服务器内部错误"
                });
            }
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="dto">房间创建数据</param>
        [HttpPost("rooms")]
        public async Task<ActionResult<ApiResponse<RoomDetailDto>>> CreateRoom([FromBody] RoomCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _roomManagementService.CreateRoomAsync(dto);
                
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetRoom), new { id = result.Data?.RoomId }, result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建房间失败");
                return StatusCode(500, new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = "服务器内部错误"
                });
            }
        }

        /// <summary>
        /// 更新房间
        /// </summary>
        /// <param name="id">房间ID</param>
        /// <param name="dto">房间更新数据</param>
        [HttpPut("rooms/{id}")]
        public async Task<ActionResult<ApiResponse<RoomDetailDto>>> UpdateRoom(int id, [FromBody] RoomUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _roomManagementService.UpdateRoomAsync(id, dto);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新房间失败，房间ID: {RoomId}", id);
                return StatusCode(500, new ApiResponse<RoomDetailDto>
                {
                    Success = false,
                    Message = "服务器内部错误"
                });
            }
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="id">房间ID</param>
        [HttpDelete("rooms/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRoom(int id)
        {
            try
            {
                var result = await _roomManagementService.DeleteRoomAsync(id);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除房间失败，房间ID: {RoomId}", id);
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "服务器内部错误",
                    Data = false
                });
            }
        }

        /// <summary>
        /// 获取房间统计信息
        /// </summary>
        [HttpGet("rooms/statistics")]
        public async Task<ActionResult<ApiResponse<object>>> GetRoomStatistics()
        {
            try
            {
                var result = await _roomManagementService.GetRoomStatisticsAsync();
                
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取房间统计失败");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "服务器内部错误"
                });
            }
        }
    }
}
