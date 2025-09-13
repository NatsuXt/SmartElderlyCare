using System.Data;
using ElderCare.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace ElderCare.Api.Modules.Medical
{
    [ApiController]
    [Route("api/medical/orders")]
    public class MedicalOrdersController : ControllerBase
    {
        private readonly IDbConnectionFactory _factory;
        private readonly IMedicalOrdersRepository _repo;

        public MedicalOrdersController(IDbConnectionFactory factory, IMedicalOrdersRepository repo)
        {
            _factory = factory;
            _repo = repo;
        }

        // POST: 新增医嘱
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateMedicalOrderDto dto)
        {
            if (dto is null) return BadRequest(new { message = "请求体为空" });

            const int maxRetry = 2;
            for (int attempt = 0; attempt <= maxRetry; attempt++)
            {
                using var conn = _factory.Create();
                if (conn.State == ConnectionState.Broken) { try { conn.Close(); } catch { } }
                if (conn.State != ConnectionState.Open) conn.Open();

                using var tx = conn.BeginTransaction();
                try
                {
                    var newId = await _repo.CreateAsync(dto, tx);
                    tx.Commit();
                    return Ok(newId);
                }
                catch (OracleException ex) when (ex.Number == 12570 || ex.Number == 12571 || ex.Number == 3113 || ex.Number == 3114)
                {
                    try { tx.Rollback(); } catch { }
                    if (attempt == maxRetry) throw;
                    await Task.Delay(200 * (attempt + 1));
                    continue;
                }
                catch
                {
                    try { tx.Rollback(); } catch { }
                    throw;
                }
            }
            return StatusCode(500, new { message = "创建医嘱失败（未知原因）" });
        }

        // GET: 分页查询（可选按老人ID & 时间范围）
        // GET /api/medical/orders?elderly_id=&from=&to=&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MedicalOrder>>> GetPaged(
            [FromQuery] int? elderly_id,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var list = await _repo.QueryAsync(elderly_id, from, to, page, pageSize);
            return Ok(list);
        }

        // GET: 按老人ID查询（底层仍用分页查询）
        // GET /api/medical/orders/by-elderly/61?from=&to=&page=1&pageSize=20
        [HttpGet("by-elderly/{elderly_id:int}")]
        public async Task<ActionResult<IReadOnlyList<MedicalOrder>>> GetByElderly(
            [FromRoute] int elderly_id,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var list = await _repo.QueryAsync(elderly_id, from, to, page, pageSize);
            return Ok(list);
        }

        // GET: 单条
        // GET /api/medical/orders/123
        [HttpGet("{order_id:int}")]
        public async Task<ActionResult<MedicalOrder?>> GetOne([FromRoute] int order_id)
        {
            var mo = await _repo.GetAsync(order_id);
            if (mo is null) return NotFound(new { message = "医嘱不存在" });
            return Ok(mo);
        }

        // DELETE: 按主键删除
        // DELETE /api/medical/orders/123
        [HttpDelete("{order_id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int order_id)
        {
            const int maxRetry = 2;
            for (int attempt = 0; attempt <= maxRetry; attempt++)
            {
                using var conn = _factory.Create();
                if (conn.State == ConnectionState.Broken) { try { conn.Close(); } catch { } }
                if (conn.State != ConnectionState.Open) conn.Open();

                using var tx = conn.BeginTransaction();
                try
                {
                    var rows = await _repo.DeleteAsync(order_id, tx);
                    tx.Commit();
                    if (rows == 0) return NotFound(new { message = "医嘱不存在或已删除" });
                    return NoContent();
                }
                catch (OracleException ex) when (ex.Number == 12570 || ex.Number == 12571 || ex.Number == 3113 || ex.Number == 3114)
                {
                    try { tx.Rollback(); } catch { }
                    if (attempt == maxRetry) throw;
                    await Task.Delay(200 * (attempt + 1));
                    continue;
                }
                catch
                {
                    try { tx.Rollback(); } catch { }
                    throw;
                }
            }
            return StatusCode(500, new { message = "删除医嘱失败（未知原因）" });
        }
    }
}
