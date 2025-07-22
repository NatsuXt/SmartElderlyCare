using System;
using System.Threading.Tasks;
using ElderlyCareSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElderlyCareSystem.Controllers
{
    [ApiController]
    [Route("api/family")]
    public class FamilyQueryController : ControllerBase
    {
        private readonly FamilyQueryService _familyQueryService;

        public FamilyQueryController(FamilyQueryService familyQueryService)
        {
            _familyQueryService = familyQueryService;
        }

        /// <summary>
        /// （无需登录）根据老人ID和查询类型查询老人相关信息
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <param name="queryType">查询类型："ElderlyInfo", "HealthMonitoring", "FeeSettlement", "ActivityParticipation"</param>
        [AllowAnonymous]
        [HttpGet("{elderlyId}/query")]
        public async Task<IActionResult> QueryElderlyInfo(int elderlyId, [FromQuery] string queryType)
        {
            if (string.IsNullOrWhiteSpace(queryType))
                return BadRequest("查询类型不能为空");

            try
            {
                var result = await _familyQueryService.QueryByTypeAsync(elderlyId, queryType);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "服务器内部错误");
            }
        }
    }
}
