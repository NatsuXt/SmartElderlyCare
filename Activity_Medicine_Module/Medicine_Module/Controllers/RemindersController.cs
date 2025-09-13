using System.Data;
using Dapper;
using ElderCare.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ElderCare.Api.Modules.Medical
{
    [ApiController]
    [Route("api/medical/[controller]")]
    public class RemindersController : ControllerBase
    {
        private readonly ReminderRepository _repo;

        public RemindersController(ReminderRepository repo) => _repo = repo;

        /// <summary>
        /// 新建提醒：状态固定为“待提醒”
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateReminderDto dto)
        {
            var id = await _repo.CreateAsync(dto);
            return Ok(id);
        }

        /// <summary>
        /// 将此提醒标记为“已服药”
        /// </summary>
        [HttpPut("{reminder_id:int}/status")]
        public async Task<IActionResult> MarkTaken([FromRoute] int reminder_id)
        {
            var rows = await _repo.MarkTakenAsync(reminder_id);
            if (rows == 0) return NotFound(new { message = "提醒不存在或无需更新" });
            return NoContent();
        }

        /// <summary>
        /// 根据医嘱ID查询其下的全部提醒
        /// </summary>
        [HttpGet("by-order/{order_id:int}")]
        public async Task<ActionResult<IReadOnlyList<ReminderItemDto>>> ByOrder([FromRoute] int order_id)
        {
            var list = await _repo.ListByOrderAsync(order_id);
            return Ok(list);
        }

        /// <summary>
        /// 删除一条提醒
        /// </summary>
        [HttpDelete("{reminder_id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int reminder_id)
        {
            var rows = await _repo.DeleteAsync(reminder_id);
            if (rows == 0) return NotFound(new { message = "要删除的提醒不存在" });
            return NoContent();
        }
    }
}
