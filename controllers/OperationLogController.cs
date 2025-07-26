using ElderlyCare.Models;
using ElderlyCare.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCare.Controllers
{
    [Route("api/operation-logs")]
    [ApiController]
    public class OperationLogController : ControllerBase
    {
        private readonly IOperationLogService _operationLogService;

        public OperationLogController(IOperationLogService operationLogService)
        {
            _operationLogService = operationLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationLog>>> GetOperationLogs(
            [FromQuery] int? staffId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var logs = await _operationLogService.GetOperationLogsAsync(staffId, startDate, endDate);
            return Ok(logs);
        }

        [HttpGet("recent/{count}")]
        public async Task<ActionResult<IEnumerable<OperationLog>>> GetRecentOperationLogs(int count)
        {
            var logs = await _operationLogService.GetRecentOperationLogsAsync(count);
            return Ok(logs);
        }
    }
}