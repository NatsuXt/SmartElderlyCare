using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Services;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthDataController : ControllerBase
    {
        private readonly HealthDataService _healthDataService;
        private readonly ILogger<HealthDataController> _logger;

        public HealthDataController(HealthDataService healthDataService, ILogger<HealthDataController> logger)
        {
            _healthDataService = healthDataService;
            _logger = logger;
        }

        /// <summary>
        /// 获取健康监测数据列表（支持分页和筛选）
        /// </summary>
        /// <param name="page">页码，默认为1</param>
        /// <param name="pageSize">每页条数，默认为20</param>
        /// <param name="elderlyId">老人ID筛选</param>
        /// <param name="startDate">开始日期筛选</param>
        /// <param name="endDate">结束日期筛选</param>
        /// <param name="sortBy">排序字段（monitoringDate, elderlyId）</param>
        /// <param name="sortDesc">是否降序排列</param>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<HealthDataDetailDto>>>> GetHealthData(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? elderlyId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
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

            var result = await _healthDataService.GetHealthDataAsync(request, elderlyId, startDate, endDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 创建健康监测数据
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<HealthDataDetailDto>>> CreateHealthData([FromBody] HealthDataCreateDto dto)
        {
            var result = await _healthDataService.CreateHealthDataAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 删除健康监测数据
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteHealthData(int id)
        {
            var result = await _healthDataService.DeleteHealthDataAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
