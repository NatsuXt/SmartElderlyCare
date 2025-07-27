// Controllers/StaffInfoController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElderlyCareManagement.DTOs;
using ElderlyCareManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElderlyCareManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffInfoController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly INursingScheduleService _nursingScheduleService;
        private readonly IEmergencySosService _emergencySosService;
        private readonly IDisinfectionService _disinfectionService;

        public StaffInfoController(
            IStaffService staffService,
            INursingScheduleService nursingScheduleService,
            IEmergencySosService emergencySosService,
            IDisinfectionService disinfectionService)
        {
            _staffService = staffService;
            _nursingScheduleService = nursingScheduleService;
            _emergencySosService = emergencySosService;
            _disinfectionService = disinfectionService;
        }

        #region 员工基本信息管理

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffInfoDTO>>> GetStaffInfos()
        {
            return await _staffService.GetAllStaffAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDetailDTO>> GetStaffInfo(int id)
        {
            var staffInfo = await _staffService.GetStaffByIdAsync(id);
            if (staffInfo == null) return NotFound();
            return staffInfo;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaffInfo(int id, StaffUpdateDTO staffDto)
        {
            if (id != staffDto.StaffId) return BadRequest();
            
            try
            {
                await _staffService.UpdateStaffAsync(staffDto);
            }
            catch (Exception)
            {
                if (await _staffService.GetStaffByIdAsync(id) == null)
                    return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<StaffInfoDTO>> PostStaffInfo(StaffCreateDTO staffDto)
        {
            var staffInfo = await _staffService.AddStaffAsync(staffDto);
            return CreatedAtAction("GetStaffInfo", new { id = staffInfo.StaffId }, staffInfo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaffInfo(int id)
        {
            var staffInfo = await _staffService.GetStaffByIdAsync(id);
            if (staffInfo == null) return NotFound();
            
            await _staffService.DeleteStaffAsync(id);
            return NoContent();
        }

        #endregion

        #region 护理计划智能排班

        [HttpGet("NursingPlans/Unassigned")]
        public async Task<ActionResult<IEnumerable<NursingPlanDTO>>> GetUnassignedNursingPlans()
        {
            return await _nursingScheduleService.GetUnassignedNursingPlansAsync();
        }

        [HttpGet("NursingPlans/{planId}")]
        public async Task<ActionResult<NursingPlanDetailDTO>> GetNursingPlanDetail(int planId)
        {
            var plan = await _nursingScheduleService.GetNursingPlanDetailAsync(planId);
            if (plan == null) return NotFound();
            return plan;
        }

        [HttpPost("NursingPlans/AutoSchedule")]
        public async Task<IActionResult> AutoScheduleNursingPlans()
        {
            await _nursingScheduleService.AutoScheduleNursingPlansAsync();
            return NoContent();
        }

        [HttpPut("NursingPlans/Assign/{nursingPlanId}")]
        public async Task<IActionResult> AssignStaffToNursingPlan(int nursingPlanId, [FromBody] int staffId)
        {
            await _nursingScheduleService.AssignStaffToNursingPlanAsync(nursingPlanId, staffId);
            return NoContent();
        }

        #endregion

        #region 紧急SOS呼叫处理

        [HttpGet("EmergencySOS/Pending")]
        public async Task<ActionResult<IEnumerable<EmergencySosDTO>>> GetPendingSosCalls()
        {
            return await _emergencySosService.GetPendingSosCallsAsync();
        }

        [HttpGet("EmergencySOS/{callId}")]
        public async Task<ActionResult<EmergencySosDetailDTO>> GetSosCallDetail(int callId)
        {
            var sosCall = await _emergencySosService.GetSosCallDetailAsync(callId);
            if (sosCall == null) return NotFound();
            return sosCall;
        }

        [HttpPost("EmergencySOS")]
        public async Task<ActionResult<EmergencySosDTO>> CreateEmergencySos([FromBody] EmergencySosCreateDTO request)
        {
            var sosCall = await _emergencySosService.CreateEmergencySosAsync(request);
            return CreatedAtAction("GetPendingSosCalls", sosCall);
        }

        [HttpPut("EmergencySOS/Assign/{callId}")]
        public async Task<IActionResult> AssignResponderToSos(int callId, [FromBody] int staffId)
        {
            await _emergencySosService.AssignResponderToSosAsync(callId, staffId);
            return NoContent();
        }

        [HttpPut("EmergencySOS/Complete/{callId}")]
        public async Task<IActionResult> CompleteSosCall(int callId, [FromBody] EmergencySosUpdateDTO request)
        {
            if (callId != request.CallId) return BadRequest();
            
            await _emergencySosService.CompleteSosCallAsync(request);
            return NoContent();
        }

        #endregion

        #region 消毒记录管理

        [HttpGet("DisinfectionRecords")]
        public async Task<ActionResult<IEnumerable<DisinfectionRecordDTO>>> GetDisinfectionRecords(
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            return await _disinfectionService.GetDisinfectionRecordsAsync(startDate, endDate);
        }

        [HttpGet("DisinfectionRecords/{recordId}")]
        public async Task<ActionResult<DisinfectionRecordDetailDTO>> GetDisinfectionRecordDetail(int recordId)
        {
            var record = await _disinfectionService.GetDisinfectionRecordDetailAsync(recordId);
            if (record == null) return NotFound();
            return record;
        }

        [HttpPost("DisinfectionRecords")]
        public async Task<ActionResult<DisinfectionRecordDTO>> RecordDisinfection([FromBody] DisinfectionCreateDTO request)
        {
            var record = await _disinfectionService.RecordDisinfectionAsync(request);
            return CreatedAtAction("GetDisinfectionRecords", new { id = record.DisinfectionId }, record);
        }

        [HttpGet("DisinfectionReports/{year}/{month}")]
        public async Task<ActionResult<DisinfectionReportDTO>> GetMonthlyDisinfectionReport(int year, int month)
        {
            return await _disinfectionService.GenerateMonthlyReportAsync(year, month);
        }

        #endregion
    }
}