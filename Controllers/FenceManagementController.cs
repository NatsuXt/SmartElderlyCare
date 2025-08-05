using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FenceManagementController : ControllerBase
    {
        private readonly FenceManagementService _fenceManagementService;
        private readonly ILogger<FenceManagementController> _logger;

        public FenceManagementController(FenceManagementService fenceManagementService, ILogger<FenceManagementController> logger)
        {
            _fenceManagementService = fenceManagementService;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有电子围栏
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<FenceDetailDto>>>> GetFences()
        {
            var request = new PagedRequest(); // 使用默认参数
            var result = await _fenceManagementService.GetFencesAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 根据ID获取电子围栏详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FenceDetailDto>>> GetFence(int id)
        {
            var result = await _fenceManagementService.GetFenceByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// 创建新电子围栏
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<FenceDetailDto>>> CreateFence([FromBody] FenceCreateDto dto)
        {
            var result = await _fenceManagementService.CreateFenceAsync(dto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// 更新电子围栏
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<FenceDetailDto>>> UpdateFence(int id, [FromBody] FenceUpdateDto dto)
        {
            var result = await _fenceManagementService.UpdateFenceAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 删除电子围栏
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteFence(int id)
        {
            var result = await _fenceManagementService.DeleteFenceAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
