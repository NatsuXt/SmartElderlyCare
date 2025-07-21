using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ElderlyCareSystem.Services;

namespace ElderlyCareSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ElderlyRecordController : ControllerBase
    {
        private readonly ElderlyRecordService _elderlyRecordService;

        public ElderlyRecordController(ElderlyRecordService elderlyRecordService)
        {
            _elderlyRecordService = elderlyRecordService;
        }

        [HttpGet("{elderlyId}")]
        public async Task<IActionResult> GetElderlyRecord(int elderlyId)
        {
            var record = await _elderlyRecordService.GetElderlyFullRecordAsync(elderlyId);
            if (record == null)
                return NotFound($"未找到id为{elderlyId}的老人档案。");

            return Ok(record);
        }
    }
}
