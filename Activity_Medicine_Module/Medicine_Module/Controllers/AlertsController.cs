using Microsoft.AspNetCore.Mvc;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    /// <summary>
    /// 告警接口：
    /// POST  /api/medical/alerts/check
    /// GET   /api/medical/alerts
    /// PATCH /api/medical/alerts/{id}/close
    /// </summary>
    [ApiController]
    [Route("api/medical/alerts")]
    public sealed class AlertsController : ControllerBase
    {
        private readonly AlertsService _svc;
        public AlertsController(AlertsService svc) => _svc = svc;

        [HttpPost("check")]
        public async Task<ActionResult<ApiResponse<int>>> Check([FromBody] HealthDataSampleDto dto)
        {
            var id = await _svc.CheckAsync(dto);
            return Ok(ApiResponse<int>.Success(id));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<HealthAlert>>>> List(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? elderly_id = null,
            [FromQuery] string? alert_type = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var list = await _svc.ListAsync(page, pageSize, elderly_id, alert_type, status, from, to);
            return Ok(ApiResponse<IReadOnlyList<HealthAlert>>.Success(list));
        }

        [HttpPatch("{id:int}/close")]
        public async Task<ActionResult<ApiResponse<int>>> Close([FromRoute] int id)
        {
            var rows = await _svc.CloseAsync(id);
            return Ok(ApiResponse<int>.Success(rows));
        }
    }
}
