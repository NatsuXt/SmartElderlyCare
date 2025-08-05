using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FenceLogController : ControllerBase
    {
        private readonly FenceLogService _fenceLogService;
        private readonly ILogger<FenceLogController> _logger;

        public FenceLogController(FenceLogService fenceLogService, ILogger<FenceLogController> logger)
        {
            _fenceLogService = fenceLogService;
            _logger = logger;
        }

        /// <summary>
        /// 获取围栏日志列表（支持分页和筛选）
        /// </summary>
        /// <param name="page">页码，默认为1</param>
        /// <param name="pageSize">每页条数，默认为20</param>
        /// <param name="elderlyId">老人ID筛选</param>
        /// <param name="fenceId">围栏ID筛选</param>
        /// <param name="sortBy">排序字段（entryTime, exitTime）</param>
        /// <param name="sortDesc">是否降序排列</param>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<FenceLogDetailDto>>>> GetFenceLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? elderlyId = null,
            [FromQuery] int? fenceId = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false)
        {
            var request = new PagedRequest
            {
                Page = page,
                PageSize = Math.Min(pageSize, 100),
                SortBy = sortBy,
                SortDesc = sortDesc
            };

            var result = await _fenceLogService.GetFenceLogsAsync(request, elderlyId, fenceId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 创建围栏日志记录
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<FenceLogDetailDto>>> CreateFenceLog([FromBody] FenceLogCreateDto dto)
        {
            var result = await _fenceLogService.CreateFenceLogAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
