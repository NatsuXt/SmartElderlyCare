using ElderlyCareSystem.Dtos;
using ElderlyCareSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElderlyCareSystem.Controllers
{
    [ApiController]
    [Route("api/mobile/family-auth")]
    public class FamilyAuthController : ControllerBase
    {
        private readonly FamilyAuthService _authService;

        public FamilyAuthController(FamilyAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/mobile/family-auth/validate
        [HttpPost("validate")]
        public async Task<IActionResult> ValidateLogin([FromBody] FamilyLoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest("参数错误");

            bool isValid = await _authService.ValidateFamilyLoginAsync(request);

            if (!isValid)
                return Unauthorized("登录信息不正确");

            return Ok("验证成功");
        }
    }
}