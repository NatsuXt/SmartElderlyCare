using ElderlyCare.Models;
using ElderlyCare.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCare.Controllers
{
    [Route("api/disinfection-records")]
    [ApiController]
    public class DisinfectionRecordController : ControllerBase
    {
        private readonly IDisinfectionRecordService _disinfectionService;
        private readonly IOperationLogService _operationLogService;

        public DisinfectionRecordController(
            IDisinfectionRecordService disinfectionService,
            IOperationLogService operationLogService)
        {
            _disinfectionService = disinfectionService;
            _operationLogService = operationLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisinfectionRecord>>> GetDisinfectionRecords(
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var records = await _disinfectionService.GetDisinfectionRecordsAsync(startDate, endDate);
            await LogOperation("获取消毒记录");
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DisinfectionRecord>> GetDisinfectionRecordById(int id)
        {
            var record = await _disinfectionService.GetDisinfectionRecordByIdAsync(id);
            if (record == null)
            {
                return NotFound();
            }
            await LogOperation($"获取消毒记录ID={id}");
            return Ok(record);
        }

        [HttpPost]
        public async Task<ActionResult<DisinfectionRecord>> CreateDisinfectionRecord(DisinfectionRecord record)
        {
            await _disinfectionService.CreateDisinfectionRecordAsync(record);
            await LogOperation($"创建消毒记录ID={record.DisinfectionId}");
            return CreatedAtAction(nameof(GetDisinfectionRecordById), new { id = record.DisinfectionId }, record);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDisinfectionRecord(int id)
        {
            await _disinfectionService.DeleteDisinfectionRecordAsync(id);
            await LogOperation($"删除消毒记录ID={id}");
            return NoContent();
        }

        [HttpGet("by-staff/{staffId}")]
        public async Task<ActionResult<IEnumerable<DisinfectionRecord>>> GetDisinfectionRecordsByStaffId(int staffId)
        {
            var records = await _disinfectionService.GetDisinfectionRecordsByStaffIdAsync(staffId);
            await LogOperation($"获取员工ID={staffId}的消毒记录");
            return Ok(records);
        }

        [HttpGet("report/{year}/{month}")]
        public async Task<IActionResult> GenerateMonthlyDisinfectionReport(int year, int month)
        {
            await _disinfectionService.GenerateMonthlyDisinfectionReportAsync(year, month);
            await LogOperation($"生成{year}年{month}月消毒报告");
            return NoContent();
        }

        private async Task LogOperation(string description)
        {
            await _operationLogService.LogOperationAsync(new OperationLog
            {
                OperationTime = DateTime.Now,
                OperationType = "DisinfectionRecord",
                OperationDescription = description,
                OperationStatus = "Success",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeviceType = HttpContext.Request.Headers["User-Agent"].ToString()
            });
        }
    }
}