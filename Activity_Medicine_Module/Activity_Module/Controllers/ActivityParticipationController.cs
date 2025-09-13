using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ElderCare.Api.Modules.Activities
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityParticipationController : ControllerBase
    {
        private readonly IDbConnectionFactory _factory;
        private readonly ParticipationRepository _repo;

        public ActivityParticipationController(IDbConnectionFactory factory, ParticipationRepository repo)
        {
            _factory = factory;
            _repo = repo;
        }

        // POST: 报名
        [HttpPost]
        public async Task<ActionResult<int>> Register([FromBody] ParticipationKeyDto dto)
        {
            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();   
            using var tx = conn.BeginTransaction();

            try
            {
                var id = await _repo.RegisterAsync(dto.activity_id, dto.elderly_id, tx);
                tx.Commit();
                return Ok(id);
            }
            catch (ArgumentException ex)
            {
                tx.Rollback();
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // PUT: 签到
        [HttpPut("check-in")]
        public async Task<ActionResult> CheckIn([FromBody] ParticipationKeyDto dto)
        {
            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();   // ✅
            using var tx = conn.BeginTransaction();

            try
            {
                var rows = await _repo.CheckInAsync(dto.activity_id, dto.elderly_id, tx);
                tx.Commit();
                if (rows == 0) return NotFound(new { message = "记录不存在或无需更新" });
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                tx.Rollback();
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // DELETE: 取消报名
        [HttpDelete]
        public async Task<ActionResult> Cancel([FromBody] ParticipationKeyDto dto)
        {
            using var conn = _factory.Create();
            if (conn.State != ConnectionState.Open) conn.Open();   
            using var tx = conn.BeginTransaction();

            try
            {
                var rows = await _repo.CancelAsync(dto.activity_id, dto.elderly_id, tx);
                tx.Commit();
                if (rows == 0) return NotFound(new { message = "未找到可取消的报名记录" });
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                tx.Rollback();
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // GET: 按活动
        [HttpGet("by-activity/{activity_id:int}")]
        public async Task<ActionResult<IReadOnlyList<ActivityParticipationItemDto>>> ByActivity([FromRoute] int activity_id)
        {
            var list = await _repo.ListByActivityAsync(activity_id);
            return Ok(list);
        }

        // GET: 按老人
        [HttpGet("by-elderly/{elderly_id:int}")]
        public async Task<ActionResult<IReadOnlyList<ElderlyParticipationItemDto>>> ByElderly([FromRoute] int elderly_id)
        {
            var list = await _repo.ListByElderlyAsync(elderly_id);
            return Ok(list);
        }
    }
}
