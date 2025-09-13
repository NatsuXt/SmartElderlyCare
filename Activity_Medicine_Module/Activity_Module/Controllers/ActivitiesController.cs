using Microsoft.AspNetCore.Mvc;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Activities
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ActivityController : ControllerBase
    {
        private readonly IActivitiesService _svc;
        public ActivityController(IActivitiesService svc) => _svc = svc;

        // POST /api/Activity  —— 发布活动
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateActivityDto dto)
            => Ok(ApiResponse<int>.Success(await _svc.CreateActivityAsync(dto)));

        // GET /api/Activity  —— 查看活动（支持时间区间与分页）
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<ActivitySchedule>>>> Query(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => Ok(ApiResponse<IReadOnlyList<ActivitySchedule>>.Success(
                await _svc.QueryActivitiesAsync(from, to, page, pageSize)));



        // DELETE /api/Activity/{id} —— 取消活动
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            await _svc.DeleteActivityAsync(id);
            return Ok(ApiResponse<string>.Success("deleted"));
        }
    }
}
