using Microsoft.AspNetCore.Mvc;
using ElderCare.Api.Infrastructure;

namespace ElderCare.Api.Modules.Medical
{
    [ApiController]
    [Route("api/medical/stock")]
    public sealed class StockPriceController : ControllerBase
    {
        private readonly StockPriceRepository _repo;
        private readonly ErrorHandlingMiddleware _err;

        public StockPriceController(StockPriceRepository repo)
        {
            _repo = repo;
        }

        // POST /api/medical/stock
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateStockBatchDto dto)
        {
            var id = await _repo.CreateAsync(dto);
            return Ok(ApiResponse<int>.Success(id));
        }

        // GET /api/medical/stock?page=1&pageSize=20&medicineId=..&activeOnly=1&kw=..
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<StockBatch>>>> Query(
            [FromQuery] int? medicineId, [FromQuery] int? activeOnly, [FromQuery] string? kw,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var list = await _repo.QueryAsync(medicineId, activeOnly, kw, page, pageSize);
            return Ok(ApiResponse<IReadOnlyList<StockBatch>>.Success(list));
        }

        // GET /api/medical/stock/{id}
        [HttpGet("{stock_batch_id:int}")]
        public async Task<ActionResult<ApiResponse<StockBatch?>>> GetOne([FromRoute] int stock_batch_id)
        {
            var one = await _repo.GetAsync(stock_batch_id);
            return Ok(ApiResponse<StockBatch?>.Success(one));
        }

        // PUT /api/medical/stock/{id}
        [HttpPut("{stock_batch_id:int}")]
        public async Task<ActionResult<ApiResponse<int>>> Update([FromRoute] int stock_batch_id, [FromBody] UpdateStockBatchDto dto)
        {
            var rows = await _repo.UpdateAsync(stock_batch_id, dto);
            return Ok(ApiResponse<int>.Success(rows));
        }

        // PATCH /api/medical/stock/{id}/quantity?delta=5  (正数增加、负数减少)
        /// <summary>
        /// 调整批次数量（delta 为正表示入库，负表示扣减）
        /// </summary>
        // StockPriceController.cs 片段（示例：PATCH /api/medical/stock/{stock_batch_id}/quantity?delta=...）
        [HttpPatch("stock/{stock_batch_id:int}/quantity")]
        public async Task<ActionResult<ApiResponse<int>>> AdjustQuantity(
            int stock_batch_id,
            [FromQuery] int delta,
            [FromServices] MedicalService medSvc)   // <<—— 注入服务
        {
            var ok = await _repo.AdjustQuantityAsync(stock_batch_id, delta);
            if (!ok) return ApiResponse<int>.Fail("调整失败或库存不足");

            // 关键：查出该批次的 medicine_id 用于触发自动补货
            var batch = await _repo.GetAsync(stock_batch_id);
            if (batch != null)
            {
                // staff_id 从登录/请求头取不到时，可以用 0 或系统账号占位
                await medSvc.AutoProcureIfBelowThresholdAsync(batch.medicine_id, staff_id: 0);
            }

            return ApiResponse<int>.Success(1);
        }


        // DELETE /api/medical/stock/{id}  (软删除: IS_ACTIVE=0)
        [HttpDelete("{stock_batch_id:int}")]
        public async Task<ActionResult<ApiResponse<int>>> Delete([FromRoute] int stock_batch_id)
        {
            var rows = await _repo.DeleteAsync(stock_batch_id);
            return Ok(ApiResponse<int>.Success(rows));
        }

        // GET /api/medical/stock/aggregates?medicineId=..&activeOnly=1
        [HttpGet("stock/aggregates")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<StockAggregate>>>> GetAggregates(
    [FromQuery] int? medicine_id,
    [FromQuery] int? activeOnly = 1)
        {
            var list = await _repo.GetAggregatesAsync(medicine_id, activeOnly);
            return ApiResponse<IReadOnlyList<StockAggregate>>.Success(list);
        }

    }
}
