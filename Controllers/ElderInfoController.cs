using ElderlyCareSystem.Dtos;
using ElderlyCareSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElderlyCareSystem.Controllers
{
    /// <summary>
    /// 老人信息控制器
    /// 提供对老人信息的增删改查接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ElderlyInfoController : ControllerBase
    {
        private readonly ElderlyInfoService _service;

        public ElderlyInfoController(ElderlyInfoService service)
        {
            _service = service;
        }

        /// <summary>
        /// 根据老人ID获取老人信息
        /// GET: api/ElderlyInfo/1
        /// </summary>
        [HttpGet("{elderlyId}")]
        public async Task<IActionResult> GetElderlyById(int elderlyId)
        {
            var elderly = await _service.GetElderlyByIdAsync(elderlyId);
            if (elderly == null) return NotFound("未找到该老人信息");
            return Ok(elderly);
        }

        /// <summary>
        /// 获取所有老人信息
        /// GET: api/ElderlyInfo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllElderlies()
        {
            var elderlies = await _service.GetAllElderliesAsync();
            return Ok(elderlies);
        }

        /// <summary>
        /// 修改老人信息的指定属性
        /// PATCH api/ElderlyInfo/1?propertyName=IdCardNumber&value=110105194712051234
        /// </summary>
        [HttpPatch("{elderlyId}")]
        public async Task<IActionResult> UpdateProperty(
    int elderlyId, [FromQuery] string propertyName, [FromQuery] string value)
        {
            if (string.IsNullOrEmpty(propertyName))
                return BadRequest("请提供 propertyName");

            var success = await _service.UpdatePropertyAsync(elderlyId, propertyName, value);
            if (!success)
                return BadRequest("修改失败，可能属性不存在或ID不存在");

            return Ok("修改成功");
        }



        /// <summary>
        /// 删除指定老人信息
        /// DELETE: api/ElderlyInfo/1
        /// </summary>
        [HttpDelete("{elderlyId}")]
        public async Task<IActionResult> DeleteElderly(int elderlyId)
        {
            var success = await _service.DeleteElderlyAsync(elderlyId);
            if (!success) return NotFound("未找到该老人信息");

            return Ok("删除成功");
        }

    }
}
