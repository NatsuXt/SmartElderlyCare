using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Models;
using RoomDeviceManagement.Services;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// 电子围栏API控制器
    /// </summary>
    [ApiController]
    [Route("api/electronic-fence")]
    [Produces("application/json")]
    public class ElectronicFenceApiController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public ElectronicFenceApiController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// 获取所有电子围栏
        /// </summary>
        /// <returns>电子围栏列表</returns>
        [HttpGet]
        public ActionResult<List<ElectronicFence>> GetAllFences()
        {
            try
            {
                var fences = _databaseService.GetAllFences();
                return Ok(fences);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取电子围栏失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取电子围栏
        /// </summary>
        /// <param name="fenceId">围栏ID</param>
        /// <returns>电子围栏信息</returns>
        [HttpGet("{fenceId:int}")]
        public ActionResult<ElectronicFence> GetFenceById(int fenceId)
        {
            try
            {
                var fence = _databaseService.GetFenceById(fenceId);
                if (fence == null)
                {
                    return NotFound(new { message = $"未找到ID为 {fenceId} 的电子围栏" });
                }
                return Ok(fence);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取电子围栏失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据围栏名称获取电子围栏
        /// </summary>
        /// <param name="fenceName">围栏名称</param>
        /// <returns>电子围栏列表</returns>
        [HttpGet("name/{fenceName}")]
        public ActionResult<List<ElectronicFence>> GetFencesByName(string fenceName)
        {
            try
            {
                var fences = _databaseService.GetFencesByName(fenceName);
                return Ok(fences);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "搜索电子围栏失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据围栏类型获取电子围栏
        /// </summary>
        /// <param name="fenceType">围栏类型</param>
        /// <returns>电子围栏列表</returns>
        [HttpGet("type/{fenceType}")]
        public ActionResult<List<ElectronicFence>> GetFencesByType(string fenceType)
        {
            try
            {
                var fences = _databaseService.GetFencesByType(fenceType);
                return Ok(fences);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取围栏类型失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据状态获取电子围栏
        /// </summary>
        /// <param name="status">围栏状态</param>
        /// <returns>电子围栏列表</returns>
        [HttpGet("status/{status}")]
        public ActionResult<List<ElectronicFence>> GetFencesByStatus(string status)
        {
            try
            {
                var fences = _databaseService.GetFencesByStatus(status);
                return Ok(fences);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取围栏状态失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 添加电子围栏
        /// </summary>
        /// <param name="fence">电子围栏信息</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public ActionResult AddFence([FromBody] ElectronicFence fence)
        {
            try
            {
                if (fence == null)
                {
                    return BadRequest(new { message = "电子围栏信息不能为空" });
                }

                var result = _databaseService.AddFence(fence);
                if (result)
                {
                    return Ok(new { message = "电子围栏添加成功", fenceId = fence.FenceId });
                }
                else
                {
                    return BadRequest(new { message = "电子围栏添加失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "添加电子围栏失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 更新电子围栏
        /// </summary>
        /// <param name="fenceId">围栏ID</param>
        /// <param name="fence">电子围栏信息</param>
        /// <returns>操作结果</returns>
        [HttpPut("{fenceId:int}")]
        public ActionResult UpdateFence(int fenceId, [FromBody] ElectronicFence fence)
        {
            try
            {
                if (fence == null)
                {
                    return BadRequest(new { message = "电子围栏信息不能为空" });
                }

                fence.FenceId = fenceId;
                var result = _databaseService.UpdateFence(fence);
                if (result)
                {
                    return Ok(new { message = "电子围栏更新成功" });
                }
                else
                {
                    return NotFound(new { message = "电子围栏不存在或更新失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新电子围栏失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 删除电子围栏
        /// </summary>
        /// <param name="fenceId">围栏ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{fenceId:int}")]
        public ActionResult DeleteFence(int fenceId)
        {
            try
            {
                var result = _databaseService.DeleteFence(fenceId);
                if (result)
                {
                    return Ok(new { message = "电子围栏删除成功" });
                }
                else
                {
                    return NotFound(new { message = "电子围栏不存在或删除失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "删除电子围栏失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 启用电子围栏
        /// </summary>
        /// <param name="fenceId">围栏ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{fenceId:int}/enable")]
        public ActionResult EnableFence(int fenceId)
        {
            try
            {
                var result = _databaseService.EnableFence(fenceId);
                if (result)
                {
                    return Ok(new { message = "电子围栏启用成功" });
                }
                else
                {
                    return NotFound(new { message = "电子围栏不存在或启用失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "启用电子围栏失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 禁用电子围栏
        /// </summary>
        /// <param name="fenceId">围栏ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{fenceId:int}/disable")]
        public ActionResult DisableFence(int fenceId)
        {
            try
            {
                var result = _databaseService.DisableFence(fenceId);
                if (result)
                {
                    return Ok(new { message = "电子围栏禁用成功" });
                }
                else
                {
                    return NotFound(new { message = "电子围栏不存在或禁用失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "禁用电子围栏失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取电子围栏统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        [HttpGet("statistics")]
        public ActionResult GetFenceStatistics()
        {
            try
            {
                var statistics = _databaseService.GetFenceStatistics();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取电子围栏统计失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取围栏日志
        /// </summary>
        /// <param name="fenceId">围栏ID</param>
        /// <returns>围栏日志列表</returns>
        [HttpGet("{fenceId:int}/logs")]
        public ActionResult<List<FenceLog>> GetFenceLogs(int fenceId)
        {
            try
            {
                var logs = _databaseService.GetFenceLogsByFenceId(fenceId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取围栏日志失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据日期范围获取围栏日志
        /// </summary>
        /// <param name="fenceId">围栏ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>围栏日志列表</returns>
        [HttpGet("{fenceId:int}/logs/date-range")]
        public ActionResult<List<FenceLog>> GetFenceLogsByDateRange(int fenceId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var logs = _databaseService.GetFenceLogsByDateRange(fenceId, startDate, endDate);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取日期范围围栏日志失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 检查位置是否在围栏内
        /// </summary>
        /// <param name="fenceId">围栏ID</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns>检查结果</returns>
        [HttpGet("{fenceId:int}/check-location")]
        public ActionResult CheckLocationInFence(int fenceId, [FromQuery] float latitude, [FromQuery] float longitude)
        {
            try
            {
                var result = _databaseService.CheckLocationInFence(fenceId, latitude, longitude);
                return Ok(new { fenceId, latitude, longitude, isInside = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "检查位置失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 批量添加电子围栏
        /// </summary>
        /// <param name="fences">电子围栏列表</param>
        /// <returns>操作结果</returns>
        [HttpPost("batch")]
        public ActionResult BatchAddFences([FromBody] List<ElectronicFence> fences)
        {
            try
            {
                if (fences == null || !fences.Any())
                {
                    return BadRequest(new { message = "电子围栏列表不能为空" });
                }

                var result = _databaseService.BatchAddFences(fences);
                return Ok(new { message = $"批量添加成功，共添加 {result} 个围栏" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "批量添加电子围栏失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 搜索电子围栏
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>搜索结果</returns>
        [HttpGet("search")]
        public ActionResult<List<ElectronicFence>> SearchFences([FromQuery] string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new { message = "搜索关键词不能为空" });
                }

                var fences = _databaseService.SearchFences(keyword);
                return Ok(fences);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "搜索电子围栏失败", error = ex.Message });
            }
        }
    }
}
