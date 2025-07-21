using System.Collections.Generic;
using System.Threading.Tasks;
using ElderlyFullRegistrationDto.Dtos;
using ElderlyFullRegistrationDto.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElderlyFullRegistrationDto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInController : ControllerBase
    {
        private readonly CheckInService _checkInService;

        public CheckInController(CheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterElderly([FromBody] Dtos.ElderlyFullRegistrationDto dto)
        {
            if (dto == null || dto.Elderly == null || dto.Assessment == null || dto.Monitoring == null)
            {
                return BadRequest("参数不完整");
            }

            var elderlyId = await _checkInService.RegisterElderlyAsync(
                dto.Elderly,
                dto.Assessment,
                dto.Monitoring,
                dto.Families ?? new List<FamilyDto>()
            );

            return Ok(new { ElderlyId = elderlyId, Message = "老人入住登记成功" });
        }
    }
}
