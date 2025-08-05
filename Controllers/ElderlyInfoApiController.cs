using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Models;
using RoomDeviceManagement.Services;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// 老人信息API控制器
    /// </summary>
    [ApiController]
    [Route("api/elderly")]
    [Produces("application/json")]
    public class ElderlyInfoApiController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public ElderlyInfoApiController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// 获取所有老人信息
        /// </summary>
        /// <returns>老人信息列表</returns>
        [HttpGet]
        public ActionResult<List<ElderlyInfo>> GetAllElderly()
        {
            try
            {
                var elderly = _databaseService.GetAllElderly();
                return Ok(elderly);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取老人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取老人信息
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <returns>老人信息</returns>
        [HttpGet("{elderlyId:int}")]
        public ActionResult<ElderlyInfo> GetElderlyById(int elderlyId)
        {
            try
            {
                var elderly = _databaseService.GetElderlyById(elderlyId);
                if (elderly == null)
                {
                    return NotFound(new { message = $"未找到ID为 {elderlyId} 的老人信息" });
                }
                return Ok(elderly);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取老人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据姓名搜索老人信息
        /// </summary>
        /// <param name="name">老人姓名</param>
        /// <returns>老人信息列表</returns>
        [HttpGet("search/name/{name}")]
        public ActionResult<List<ElderlyInfo>> GetElderlyByName(string name)
        {
            try
            {
                var elderly = _databaseService.GetElderlyByName(name);
                return Ok(elderly);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "搜索老人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据房间号获取老人信息
        /// </summary>
        /// <param name="roomNumber">房间号</param>
        /// <returns>老人信息列表</returns>
        [HttpGet("room/{roomNumber}")]
        public ActionResult<List<ElderlyInfo>> GetElderlyByRoom(string roomNumber)
        {
            try
            {
                var elderly = _databaseService.GetElderlyByRoom(roomNumber);
                return Ok(elderly);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取房间老人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据年龄范围获取老人信息
        /// </summary>
        /// <param name="minAge">最小年龄</param>
        /// <param name="maxAge">最大年龄</param>
        /// <returns>老人信息列表</returns>
        [HttpGet("age-range")]
        public ActionResult<List<ElderlyInfo>> GetElderlyByAgeRange([FromQuery] int minAge, [FromQuery] int maxAge)
        {
            try
            {
                var elderly = _databaseService.GetElderlyByAgeRange(minAge, maxAge);
                return Ok(elderly);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取年龄范围老人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据健康状态获取老人信息
        /// </summary>
        /// <param name="healthStatus">健康状态</param>
        /// <returns>老人信息列表</returns>
        [HttpGet("health-status/{healthStatus}")]
        public ActionResult<List<ElderlyInfo>> GetElderlyByHealthStatus(string healthStatus)
        {
            try
            {
                var elderly = _databaseService.GetElderlyByHealthStatus(healthStatus);
                return Ok(elderly);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取健康状态老人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 添加老人信息
        /// </summary>
        /// <param name="elderly">老人信息</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public ActionResult AddElderly([FromBody] ElderlyInfo elderly)
        {
            try
            {
                if (elderly == null)
                {
                    return BadRequest(new { message = "老人信息不能为空" });
                }

                var result = _databaseService.AddElderly(elderly);
                if (result)
                {
                    return Ok(new { message = "老人信息添加成功", elderlyId = elderly.ElderlyId });
                }
                else
                {
                    return BadRequest(new { message = "老人信息添加失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "添加老人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 更新老人信息
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <param name="elderly">老人信息</param>
        /// <returns>操作结果</returns>
        [HttpPut("{elderlyId:int}")]
        public ActionResult UpdateElderly(int elderlyId, [FromBody] ElderlyInfo elderly)
        {
            try
            {
                if (elderly == null)
                {
                    return BadRequest(new { message = "老人信息不能为空" });
                }

                elderly.ElderlyId = elderlyId;
                var result = _databaseService.UpdateElderly(elderly);
                if (result)
                {
                    return Ok(new { message = "老人信息更新成功" });
                }
                else
                {
                    return NotFound(new { message = "老人信息不存在或更新失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新老人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 删除老人信息
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{elderlyId:int}")]
        public ActionResult DeleteElderly(int elderlyId)
        {
            try
            {
                var result = _databaseService.DeleteElderly(elderlyId);
                if (result)
                {
                    return Ok(new { message = "老人信息删除成功" });
                }
                else
                {
                    return NotFound(new { message = "老人信息不存在或删除失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "删除老人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取老人信息统计
        /// </summary>
        /// <returns>统计信息</returns>
        [HttpGet("statistics")]
        public ActionResult GetElderlyStatistics()
        {
            try
            {
                var statistics = _databaseService.GetElderlyStatistics();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取老人信息统计失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取紧急联系人信息
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <returns>紧急联系人信息</returns>
        [HttpGet("{elderlyId:int}/emergency-contact")]
        public ActionResult GetEmergencyContact(int elderlyId)
        {
            try
            {
                var elderly = _databaseService.GetElderlyById(elderlyId);
                if (elderly == null)
                {
                    return NotFound(new { message = $"未找到ID为 {elderlyId} 的老人信息" });
                }

                var emergencyContact = new
                {
                    ElderlyId = elderly.ElderlyId,
                    ElderlyName = elderly.Name,
                    EmergencyContact = elderly.EmergencyContact,
                    EmergencyPhone = elderly.EmergencyPhone
                };

                return Ok(emergencyContact);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取紧急联系人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 更新紧急联系人信息
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <param name="contactInfo">联系人信息</param>
        /// <returns>操作结果</returns>
        [HttpPut("{elderlyId:int}/emergency-contact")]
        public ActionResult UpdateEmergencyContact(int elderlyId, [FromBody] dynamic contactInfo)
        {
            try
            {
                var elderly = _databaseService.GetElderlyById(elderlyId);
                if (elderly == null)
                {
                    return NotFound(new { message = $"未找到ID为 {elderlyId} 的老人信息" });
                }

                elderly.EmergencyContact = contactInfo.emergencyContact;
                elderly.EmergencyPhone = contactInfo.emergencyPhone;

                var result = _databaseService.UpdateElderly(elderly);
                if (result)
                {
                    return Ok(new { message = "紧急联系人信息更新成功" });
                }
                else
                {
                    return BadRequest(new { message = "紧急联系人信息更新失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新紧急联系人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 批量导入老人信息
        /// </summary>
        /// <param name="elderlyList">老人信息列表</param>
        /// <returns>操作结果</returns>
        [HttpPost("batch")]
        public ActionResult BatchAddElderly([FromBody] List<ElderlyInfo> elderlyList)
        {
            try
            {
                if (elderlyList == null || !elderlyList.Any())
                {
                    return BadRequest(new { message = "老人信息列表不能为空" });
                }

                var result = _databaseService.BatchAddElderly(elderlyList);
                return Ok(new { message = $"批量导入成功，共导入 {result} 条记录" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "批量导入老人信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 搜索老人信息
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>搜索结果</returns>
        [HttpGet("search")]
        public ActionResult<List<ElderlyInfo>> SearchElderly([FromQuery] string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new { message = "搜索关键词不能为空" });
                }

                var elderly = _databaseService.SearchElderly(keyword);
                return Ok(elderly);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "搜索老人信息失败", error = ex.Message });
            }
        }
    }
}
