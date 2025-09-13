using Microsoft.AspNetCore.Mvc;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    [ApiController]
    [Route("api/medical/dispense")]
    public sealed class DispenseController : ControllerBase
    {
        private readonly DispenseRepository _repo;
        public DispenseController(DispenseRepository repo) => _repo = repo;

        // ① 创建发药单 & 预占库存（开药）
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateDispenseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<string>.Fail("参数错误"));
            var id = await _repo.CreateReserveAsync(dto);
            return Ok(ApiResponse<int>.Success(id));
        }

        /// <summary>
        /// ② 修改支付状态（PAYMENT_STATUS）
        /// PUT /api/medical/dispense/{dispense_id}/pay
        /// body: { "pay_status":"PAID" } // UNPAID/PAID/REFUNDED 等
        /// </summary>
        [HttpPut("{dispense_id:int}/pay")]
        public async Task<ActionResult<ApiResponse<int>>> UpdatePayStatus(
            int dispense_id, [FromBody] UpdatePayStatusDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Pay_Status))
                return BadRequest(ApiResponse<int>.Fail("pay_status is required."));

            var rows = await _repo.UpdatePayStatusAsync(dispense_id, dto.Pay_Status);
            return Ok(ApiResponse<int>.Success(rows));
        }

        // ③ 确认发药（从预占扣真实库存）
        [HttpPut("{dispense_id:int}/confirm")]
        public async Task<ActionResult<ApiResponse<string>>> Confirm([FromRoute] int dispense_id)
        {
            await _repo.ConfirmAsync(dispense_id);
            return Ok(ApiResponse<string>.Success("ok"));
        }

        // ④ 取消发药（释放预占）
        [HttpPut("{dispense_id:int}/cancel")]
        public async Task<ActionResult<ApiResponse<string>>> Cancel([FromRoute] int dispense_id)
        {
            await _repo.CancelAsync(dispense_id);
            return Ok(ApiResponse<string>.Success("ok"));
        }

        // ⑤ 分页列表
        /// 分页列表（支持按 status / pay_status 等筛选）
        /// GET /api/medical/dispense?page=1&pageSize=20&elderly_id=&medicine_id=&status=&pay_status=
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<DispenseRecord>>>> Query(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? elderly_id = null,
            [FromQuery] int? medicine_id = null,
            [FromQuery] string? status = null,
            [FromQuery] string? pay_status = null)
        {
            var list = await _repo.ListAsync(page, pageSize, elderly_id, medicine_id, status, pay_status);
            return Ok(ApiResponse<IReadOnlyList<DispenseRecord>>.Success(list));
        }

        /// ⑥ 单条
        /// GET /api/medical/dispense/{dispense_id}
        /// </summary>
        [HttpGet("{dispense_id:int}")]
        public async Task<ActionResult<ApiResponse<DispenseRecord?>>> GetOne(int dispense_id)
        {
            var row = await _repo.GetAsync(dispense_id);
            return Ok(ApiResponse<DispenseRecord?>.Success(row));
        }
    }
}
