using ElderCare.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ElderCare.Api.Modules.Medical
{
    public sealed class CreateProcurementDto
    {
        [Required] public int medicine_id { get; set; }
        [Range(1, int.MaxValue)] public int quantity { get; set; }
        [Required] public int staff_id { get; set; }
        // 这里不接收 status，仓储里固定写入“待采购”
    }

    [ApiController]
    [Route("api/medical/procurements")]
    public sealed class ProcurementsController : ControllerBase
    {
        private readonly ProcurementRepository _repo;

        public ProcurementsController(ProcurementRepository repo) => _repo = repo;

        /// <summary>新增采购单：status 固定为“待采购”</summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateProcurementDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("参数无效"));

            var id = await _repo.CreateAsync(dto.medicine_id, dto.quantity, dto.staff_id, null);
            return ApiResponse<int>.Success(id);
        }

        /// <summary>将采购单置为“已入库”</summary>
        [HttpPut("{procurement_id:int}/receive")]
        public async Task<ActionResult<ApiResponse<int>>> Receive([FromRoute] int procurement_id)
        {
            var rows = await _repo.ReceiveAsync(procurement_id, null);
            return ApiResponse<int>.Success(rows);
        }

        /// <summary>分页查询，支持按状态过滤（待采购/已入库）</summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<MedicineProcurement>>>> List(
            [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var list = await _repo.ListAsync(status, page, pageSize);
            return ApiResponse<IReadOnlyList<MedicineProcurement>>.Success(list);
        }
    }
}
