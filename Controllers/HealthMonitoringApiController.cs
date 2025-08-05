using Microsoft.AspNetCore.Mvc;
using RoomDeviceManagement.Interfaces;
using RoomDeviceManagement.Models;

namespace RoomDeviceManagement.Controllers
{
    /// <summary>
    /// 健康监测API控制器
    /// </summary>
    [ApiController]
    [Route("api/health-monitoring")]
    [Produces("application/json")]
    public class HealthMonitoringApiController : ControllerBase
    {
        private readonly IHealthMonitoringService _healthService;

        public HealthMonitoringApiController(IHealthMonitoringService healthService)
        {
            _healthService = healthService;
        }

        /// <summary>
        /// 获取所有健康监测记录
        /// </summary>
        /// <returns>健康监测记录列表</returns>
        [HttpGet]
        public ActionResult<List<HealthMonitoring>> GetAllHealthRecords()
        {
            try
            {
                var records = _healthService.GetAllHealthRecords();
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取健康监测记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据记录ID获取健康监测记录
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <returns>健康监测记录</returns>
        [HttpGet("{recordId:int}")]
        public ActionResult<HealthMonitoring> GetHealthRecordById(int recordId)
        {
            try
            {
                var record = _healthService.GetHealthRecordById(recordId);
                if (record == null)
                {
                    return NotFound(new { message = $"未找到记录ID为 {recordId} 的健康监测记录" });
                }
                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取健康监测记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据老人ID获取健康监测记录
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <returns>健康监测记录列表</returns>
        [HttpGet("elderly/{elderlyId:int}")]
        public ActionResult<List<HealthMonitoring>> GetHealthRecordsByElderlyId(int elderlyId)
        {
            try
            {
                var records = _healthService.GetHealthRecordsByElderlyId(elderlyId);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取老人健康监测记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据日期范围获取健康监测记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>健康监测记录列表</returns>
        [HttpGet("date-range")]
        public ActionResult<List<HealthMonitoring>> GetHealthRecordsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var records = _healthService.GetHealthRecordsByDateRange(startDate, endDate);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取日期范围健康监测记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据健康状态获取监测记录
        /// </summary>
        /// <param name="status">健康状态</param>
        /// <returns>健康监测记录列表</returns>
        [HttpGet("status/{status}")]
        public ActionResult<List<HealthMonitoring>> GetHealthRecordsByStatus(string status)
        {
            try
            {
                var records = _healthService.GetHealthRecordsByStatus(status);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取健康状态监测记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 添加健康监测记录
        /// </summary>
        /// <param name="record">健康监测记录</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public ActionResult AddHealthRecord([FromBody] HealthMonitoring record)
        {
            try
            {
                if (record == null)
                {
                    return BadRequest(new { message = "健康监测记录不能为空" });
                }

                var result = _healthService.AddHealthRecord(record);
                if (result)
                {
                    return Ok(new { message = "健康监测记录添加成功", recordId = record.MonitoringId });
                }
                else
                {
                    return BadRequest(new { message = "健康监测记录添加失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "添加健康监测记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 更新健康监测记录
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="record">健康监测记录</param>
        /// <returns>操作结果</returns>
        [HttpPut("{recordId:int}")]
        public ActionResult UpdateHealthRecord(int recordId, [FromBody] HealthMonitoring record)
        {
            try
            {
                if (record == null)
                {
                    return BadRequest(new { message = "健康监测记录不能为空" });
                }

                record.MonitoringId = recordId;
                var result = _healthService.UpdateHealthRecord(record);
                if (result)
                {
                    return Ok(new { message = "健康监测记录更新成功" });
                }
                else
                {
                    return NotFound(new { message = "健康监测记录不存在或更新失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新健康监测记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 删除健康监测记录
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{recordId:int}")]
        public ActionResult DeleteHealthRecord(int recordId)
        {
            try
            {
                var result = _healthService.DeleteHealthRecord(recordId);
                if (result)
                {
                    return Ok(new { message = "健康监测记录删除成功" });
                }
                else
                {
                    return NotFound(new { message = "健康监测记录不存在或删除失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "删除健康监测记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取异常健康记录
        /// </summary>
        /// <returns>异常健康记录列表</returns>
        [HttpGet("abnormal")]
        public ActionResult<List<HealthMonitoring>> GetAbnormalHealthRecords()
        {
            try
            {
                var records = _healthService.GetAbnormalHealthRecords();
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取异常健康记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取健康监测统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        [HttpGet("statistics")]
        public ActionResult GetHealthStatistics()
        {
            try
            {
                var statistics = _healthService.GetHealthStatistics();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取健康监测统计信息失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 根据老人ID获取最新健康记录
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <returns>最新健康记录</returns>
        [HttpGet("elderly/{elderlyId:int}/latest")]
        public ActionResult<HealthMonitoring> GetLatestHealthRecordByElderlyId(int elderlyId)
        {
            try
            {
                var record = _healthService.GetLatestHealthRecordByElderlyId(elderlyId);
                if (record == null)
                {
                    return NotFound(new { message = $"未找到老人ID为 {elderlyId} 的健康监测记录" });
                }
                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "获取最新健康记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 批量添加健康监测记录
        /// </summary>
        /// <param name="records">健康监测记录列表</param>
        /// <returns>操作结果</returns>
        [HttpPost("batch")]
        public ActionResult BatchAddHealthRecords([FromBody] List<HealthMonitoring> records)
        {
            try
            {
                if (records == null || !records.Any())
                {
                    return BadRequest(new { message = "健康监测记录列表不能为空" });
                }

                var result = _healthService.BatchAddHealthRecords(records);
                return Ok(new { message = $"批量添加成功，共添加 {result} 条记录" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "批量添加健康监测记录失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 搜索健康监测记录
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>搜索结果</returns>
        [HttpGet("search")]
        public ActionResult<List<HealthMonitoring>> SearchHealthRecords([FromQuery] string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new { message = "搜索关键词不能为空" });
                }

                var records = _healthService.SearchHealthRecords(keyword);
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "搜索健康监测记录失败", error = ex.Message });
            }
        }
    }
}