using ElderlyCareSystem.Dtos;
using ElderlyCareSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElderlyCareSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInServiceController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInServiceController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpPost("full-register")]
        public async Task<IActionResult> FullRegister([FromBody] ElderlyFullRegistrationDto dto)
        {
            var elderlyId = await _checkInService.FullRegisterAsync(dto);
            return Ok(new { Message = "登记成功", ElderlyId = elderlyId });
        }
    }
}
