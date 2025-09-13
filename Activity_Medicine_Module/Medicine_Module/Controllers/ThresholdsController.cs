using ElderCare.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ElderCare.Api.Modules.Medical
{
    [ApiController]
    [Route("api/medical/thresholds")]
    public sealed class ThresholdsController : ControllerBase
    {
        private readonly HealthThresholdsService _svc;
        public ThresholdsController(HealthThresholdsService svc) => _svc = svc;

        // POST /api/medical/thresholds/upsert
        [HttpPost("upsert")]
        public async Task<ActionResult<ApiResponse<int>>> Upsert(
            [FromBody] HealthThresholdsService.HealthThresholdUpsertDto dto)
        {
            var id = await _svc.UpsertAsync(dto);
            return Ok(ApiResponse<int>.Success(id));
        }

        // GET /api/medical/thresholds?page=1&pageSize=20&elderly_id=&data_type=
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<HealthThresholdsService.HealthThreshold>>>> List(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? elderly_id = null,
            [FromQuery] string? data_type = null)
        {
            var rows = await _svc.ListAsync(page, pageSize, elderly_id, data_type);
            return Ok(ApiResponse<IReadOnlyList<HealthThresholdsService.HealthThreshold>>.Success(rows));
        }

        // GET /api/medical/thresholds/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<HealthThresholdsService.HealthThreshold?>>> Get(int id)
        {
            var row = await _svc.GetAsync(id);
            return Ok(ApiResponse<HealthThresholdsService.HealthThreshold?>.Success(row));
        }

        // DELETE /api/medical/thresholds/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<int>>> Delete(int id)
        {
            var rows = await _svc.DeleteAsync(id);
            return Ok(ApiResponse<int>.Success(rows));
        }
    }
}
