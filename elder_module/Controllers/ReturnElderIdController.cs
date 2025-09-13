using ElderlyCareSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElderlyCareSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FamilyController : ControllerBase
    {
        private readonly FamilyService _familyService;

        public FamilyController(FamilyService familyService)
        {
            _familyService = familyService;
        }

        /// <summary>
        /// 通过家属ID获取老人ID
        /// </summary>
        [HttpGet("{familyId}/elderlyId")]
        public async Task<IActionResult> GetElderlyIdByFamilyId(int familyId)
        {
            var elderlyId = await _familyService.GetElderlyIdByFamilyIdAsync(familyId);

            if (elderlyId == null)
                return NotFound(new { Message = $"未找到 familyId={familyId} 对应的老人" });

            return Ok(new { ElderlyId = elderlyId });
        }
    }
}
