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
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RoomDetailDto>>>> GetRooms(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false)
        {
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

        /// <summary>
        /// 根据ID获取房间详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoomDetailDto>>> GetRoom(int id)
        {
            var result = await _roomManagementService.GetRoomByIdAsync(id);
            
            if (result.Success)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoomDetailDto>>> CreateRoom([FromBody] RoomCreateDto dto)
        {
            var result = await _roomManagementService.CreateRoomAsync(dto);
            
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetRoom), new { id = result.Data?.RoomId }, result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// 更新房间
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<RoomDetailDto>>> UpdateRoom(int id, [FromBody] RoomUpdateDto dto)
        {
            var result = await _roomManagementService.UpdateRoomAsync(id, dto);
            
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// 删除房间
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRoom(int id)
        {
            var result = await _roomManagementService.DeleteRoomAsync(id);
            
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// 获取房间统计信息
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<ApiResponse<object>>> GetRoomStatistics()
        {
            var result = await _roomManagementService.GetRoomStatisticsAsync();
            
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
