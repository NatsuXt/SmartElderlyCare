using ElderlyCareSystem.Dtos;
using ElderlyCareSystem.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElderlyCareSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ElderlyInfoController : ControllerBase
    {
        private readonly ElderlyInfoService _service;

        // 构造函数，注入 ElderlyInfoService
        public ElderlyInfoController(ElderlyInfoService service)
        {
            _service = service;
        }

        /// <summary>
        /// 根据 ID 获取单个老人的基本信息
        /// GET: api/ElderlyInfo/1
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetElderlyById(int id)
        {
            var elderly = await _service.GetElderlyByIdAsync(id);
            if (elderly == null)
                return NotFound("未找到该老人信息");
            return Ok(elderly);
        }

        /// <summary>
        /// 获取所有老人的基本信息
        /// GET: api/ElderlyInfo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllElderlies()
        {
            var elderlies = await _service.GetAllElderliesAsync();
            return Ok(elderlies);
        }

        /// <summary>
        /// 修改指定老人的单个属性值
        /// PATCH 请求示例: api/ElderlyInfo/1?propertyName=Name&value=张三
        /// </summary>
        /// <param name="id">要修改的老人的ID</param>
        /// <param name="propertyName">要修改的属性名，例如 "Name"、"ContactPhone"</param>
        /// <param name="value">修改后的新值</param>
        /// <returns>返回操作结果，如果成功返回 "修改成功"，失败返回相应提示</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProperty(int id, [FromQuery] string propertyName, [FromQuery] string value)
        {
            // 检查参数是否为空，如果属性名或值未提供，则返回 400 错误
            if (string.IsNullOrEmpty(propertyName) || value == null)
                return BadRequest("请提供 propertyName 和 value 参数");

            // 调用 Service 方法执行修改
            var success = await _service.UpdatePropertyAsync(id, propertyName, value);

            // 如果修改失败，可能是属性名不存在或者类型转换失败，返回 400 错误
            if (!success) return BadRequest("修改失败，可能属性不存在或类型不匹配");

            // 修改成功，返回 200 状态码和提示信息
            return Ok("修改成功");
        }

        /// <summary>
        /// 删除指定 ID 老人的信息
        /// DELETE: api/ElderlyInfo/1
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteElderly(int id)
        {
            var success = await _service.DeleteElderlyAsync(id);
            if (!success)
                return NotFound("未找到该老人信息");

            return Ok("删除成功");
        }
    }
}
