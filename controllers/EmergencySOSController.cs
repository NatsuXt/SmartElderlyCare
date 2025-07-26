using ElderlyCare.Models;
using ElderlyCare.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCare.Controllers
{
    [Route("api/emergency-sos")]
    [ApiController]
    public class EmergencySOSController : ControllerBase
    {
        private readonly IEmergencySOSService _emergencySOSService;
        private readonly IOperationLogService _operationLogService;

        public EmergencySOSController(
            IEmergencySOSService emergencySOSService,
            IOperationLogService operationLogService)
        {
            _emergencySOSService = emergencySOSService;
            _operationLogService = operationLogService;
        }

        [HttpPost]
        public async Task<ActionResult<EmergencySOS>> CreateEmergencyCall(EmergencySOS emergencyCall)
        {
            var call = await _emergencySOSService.CreateEmergencyCallAsync(emergencyCall);
            await LogOperation($"创建紧急呼叫ID={call.CallId}");
            return CreatedAtAction(nameof(GetEmergencyCallById), new { id = call.CallId }, call);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmergencySOS>> GetEmergencyCallById(int id)
        {
            var call = await _emergencySOSService.GetEmergencyCallsByStaffIdAsync(id);
            if (call == null)
            {
                return NotFound();
            }
            await LogOperation($"获取紧急呼叫ID={id}");
            return Ok(call);
        }

        [HttpPost("respond/{callId}/{staffId}")]
        public async Task<IActionResult> RespondToEmergencyCall(int callId, int staffId)
        {
            await _emergencySOSService.RespondToEmergencyCallAsync(callId, staffId);
            await LogOperation($"员工ID={staffId}响应紧急呼叫ID={callId}");
            return NoContent();
        }

        [HttpPost("complete/{callId}")]
        public async Task<IActionResult> CompleteEmergencyCall(int callId, [FromBody] string handlingResult)
        {
            await _emergencySOSService.CompleteEmergencyCallAsync(callId, handlingResult);
            await LogOperation($"完成紧急呼叫ID={callId}");
            return NoContent();
        }

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<EmergencySOS>>> GetPendingEmergencyCalls()
        {
            var calls = await _emergencySOSService.GetPendingEmergencyCallsAsync();
            await LogOperation("获取待处理的紧急呼叫");
            return Ok(calls);
        }

        [HttpGet("by-staff/{staffId}")]
        public async Task<ActionResult<IEnumerable<EmergencySOS>>> GetEmergencyCallsByStaffId(int staffId)
        {
            var calls = await _emergencySOSService.GetEmergencyCallsByStaffIdAsync(staffId);
            await LogOperation($"获取员工ID={staffId}处理的紧急呼叫");
            return Ok(calls);
        }

        [HttpGet("by-elderly/{elderlyId}")]
        public async Task<ActionResult<IEnumerable<EmergencySOS>>> GetEmergencyCallsByElderlyId(int elderlyId)
        {
            var calls = await _emergencySOSService.GetEmergencyCallsByElderlyIdAsync(elderlyId);
            await LogOperation($"获取老人ID={elderlyId}的紧急呼叫记录");
            return Ok(calls);
        }

        private async Task LogOperation(string description)
        {
            await _operationLogService.LogOperationAsync(new OperationLog
            {
                OperationTime = DateTime.Now,
                OperationType = "EmergencySOS",
                OperationDescription = description,
                OperationStatus = "Success",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeviceType = HttpContext.Request.Headers["User-Agent"].ToString()
            });
        }
    }
}