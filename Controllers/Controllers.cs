using Microsoft.AspNetCore.Mvc;
using Staff_Info.Data;
using Staff_Info.DTOs;
using Staff_Info.Models;
using Staff_Info.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Staff_Info.Controllers
{
    [ApiController]
    [Route("api/staff-info")]
    public class StaffInfoController : ControllerBase
    {
        private readonly StaffInfoDbContext _context;
        private readonly INursingSchedulingService _nursingSchedulingService;
        private readonly IEmergencySOSService _emergencySOSService;
        private readonly IDisinfectionService _disinfectionService;

        public StaffInfoController(
            StaffInfoDbContext context,
            INursingSchedulingService nursingSchedulingService,
            IEmergencySOSService emergencySOSService,
            IDisinfectionService disinfectionService)
        {
            _context = context;
            _nursingSchedulingService = nursingSchedulingService;
            _emergencySOSService = emergencySOSService;
            _disinfectionService = disinfectionService;
        }

        // 员工信息管理 - 1.1.1 员工信息维护
        [HttpGet("staff")]
        public async Task<ActionResult<IEnumerable<StaffInfoDto>>> GetAllStaff()
        {
            var staff = await _context.StaffInfos
                .Select(s => new StaffInfoDto
                {
                    StaffId = s.STAFF_ID,
                    Name = s.NAME,
                    Position = s.POSITION,
                    ContactPhone = s.CONTACT_PHONE,
                    SkillLevel = s.SKILL_LEVEL,
                    WorkSchedule = s.WORK_SCHEDULE
                })
                .ToListAsync();
                
            return Ok(staff);
        }

        [HttpGet("staff/{id}")]
        public async Task<ActionResult<StaffInfoDto>> GetStaff(decimal id)
        {
            var staff = await _context.StaffInfos
                .Where(s => s.STAFF_ID == id)
                .Select(s => new StaffInfoDto
                {
                    StaffId = s.STAFF_ID,
                    Name = s.NAME,
                    Position = s.POSITION,
                    ContactPhone = s.CONTACT_PHONE,
                    SkillLevel = s.SKILL_LEVEL,
                    WorkSchedule = s.WORK_SCHEDULE
                })
                .FirstOrDefaultAsync();
                
            if (staff == null) return NotFound();
            return Ok(staff);
        }

        [HttpPost("staff")]
        public async Task<ActionResult<StaffInfoDto>> CreateStaff([FromBody] StaffInfoDto dto)
        {
            var staff = new STAFFINFO
            {
                NAME = dto.Name,
                POSITION = dto.Position,
                CONTACT_PHONE = dto.ContactPhone,
                SKILL_LEVEL = dto.SkillLevel,
                WORK_SCHEDULE = dto.WorkSchedule
            };
            
            _context.StaffInfos.Add(staff);
            await _context.SaveChangesAsync();
            
            dto.StaffId = staff.STAFF_ID;
            return CreatedAtAction(nameof(GetStaff), new { id = staff.STAFF_ID }, dto);
        }

        [HttpPut("staff/{id}")]
        public async Task<IActionResult> UpdateStaff(decimal id, [FromBody] StaffInfoDto dto)
        {
            if (id != dto.StaffId) return BadRequest("ID不匹配");
            
            var staff = await _context.StaffInfos.FindAsync(id);
            if (staff == null) return NotFound();
            
            staff.NAME = dto.Name;
            staff.POSITION = dto.Position;
            staff.CONTACT_PHONE = dto.ContactPhone;
            staff.SKILL_LEVEL = dto.SkillLevel;
            staff.WORK_SCHEDULE = dto.WorkSchedule;
            
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("staff/{id}")]
        public async Task<IActionResult> DeleteStaff(decimal id)
        {
            var staff = await _context.StaffInfos.FindAsync(id);
            if (staff == null) 
                return NotFound();
    
            // 改用 CountAsync 避免 TRUE/FALSE
            bool hasRelatedRecords =
                await _context.ActivitySchedules.CountAsync(a => a.STAFF_ID == id) > 0 ||
                await _context.DisinfectionRecords.CountAsync(d => d.STAFF_ID == id) > 0 ||
                await _context.MedicalOrders.CountAsync(m => m.STAFF_ID == id) > 0;

            if (hasRelatedRecords)
            {
                return BadRequest("无法删除员工，因为存在相关活动、消毒或医嘱记录");
            }
    
            _context.StaffInfos.Remove(staff);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        // 护理计划管理 - 1.1.4 护理计划智能排班
        [HttpPost("nursing-schedule/generate")]
        public async Task<IActionResult> GenerateNursingSchedule()
        {
            try
            {
                await _nursingSchedulingService.GenerateNursingScheduleAsync();
                return Ok(new { Message = "护理计划排班成功完成" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"排班失败: {ex.Message}");
            }
        }

        // 护理计划基础CRUD
        [HttpGet("nursing-plans")]
        public async Task<ActionResult<IEnumerable<NursingPlanDto>>> GetAllNursingPlans()
        {
            var plans = await _context.NursingPlans
                .Include(np => np.STAFF)
                .Include(np => np.ELDERLY)
                .Select(np => new NursingPlanDto
                {
                    PlanId = np.PLAN_ID,
                    ElderlyId = np.ELDERLY_ID,
                    StaffId = np.STAFF_ID,
                    PlanStartDate = np.PLAN_START_DATE,
                    PlanEndDate = np.PLAN_END_DATE,
                    CareType = np.CARE_TYPE,
                    Priority = np.PRIORITY,
                    EvaluationStatus = np.EVALUATION_STATUS
                })
                .ToListAsync();
                
            return Ok(plans);
        }

        [HttpGet("nursing-plans/{id}")]
        public async Task<ActionResult<NursingPlanDto>> GetNursingPlan(decimal id)
        {
            var plan = await _context.NursingPlans
                .Include(np => np.STAFF)
                .Include(np => np.ELDERLY)
                .Where(np => np.PLAN_ID == id)
                .Select(np => new NursingPlanDto
                {
                    PlanId = np.PLAN_ID,
                    ElderlyId = np.ELDERLY_ID,
                    StaffId = np.STAFF_ID,
                    PlanStartDate = np.PLAN_START_DATE,
                    PlanEndDate = np.PLAN_END_DATE,
                    CareType = np.CARE_TYPE,
                    Priority = np.PRIORITY,
                    EvaluationStatus = np.EVALUATION_STATUS
                })
                .FirstOrDefaultAsync();
                
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpPost("nursing-plans")]
        public async Task<ActionResult<NursingPlanDto>> CreateNursingPlan([FromBody] NursingPlanDto dto)
        {
            // 验证老人是否存在
            var elderlyExists = await _context.ElderlyInfos
                .AsNoTracking()
                .CountAsync(e => e.ELDERLY_ID == dto.ElderlyId) > 0;
            if (!elderlyExists) return BadRequest("指定的老人ID不存在");
            
            // 验证员工是否存在（如果已分配）
            if (dto.StaffId.HasValue)
            {
                var staffExists = await _context.StaffInfos.AnyAsync(s => s.STAFF_ID == dto.StaffId);
                if (!staffExists) return BadRequest("指定的员工ID不存在");
            }
            
            var plan = new NURSINGPLAN
            {
                ELDERLY_ID = dto.ElderlyId,
                STAFF_ID = dto.StaffId,
                PLAN_START_DATE = dto.PlanStartDate,
                PLAN_END_DATE = dto.PlanEndDate,
                CARE_TYPE = dto.CareType,
                PRIORITY = dto.Priority,
                EVALUATION_STATUS = dto.EvaluationStatus ?? "Pending"
            };
            
            _context.NursingPlans.Add(plan);
            await _context.SaveChangesAsync();
            
            dto.PlanId = plan.PLAN_ID;
            return CreatedAtAction(nameof(GetNursingPlan), new { id = plan.PLAN_ID }, dto);
        }

        [HttpPut("nursing-plans/{id}")]
        public async Task<IActionResult> UpdateNursingPlan(decimal id, [FromBody] NursingPlanDto dto)
        {
            if (id != dto.PlanId) return BadRequest("ID不匹配");
            
            var plan = await _context.NursingPlans.FindAsync(id);
            if (plan == null) return NotFound();
            
            // 验证老人是否存在
            var elderlyExists = await _context.ElderlyInfos.AnyAsync(e => e.ELDERLY_ID == dto.ElderlyId);
            if (!elderlyExists) return BadRequest("指定的老人ID不存在");
            
            // 验证员工是否存在（如果已分配）
            if (dto.StaffId.HasValue)
            {
                var staffExists = await _context.StaffInfos.AnyAsync(s => s.STAFF_ID == dto.StaffId);
                if (!staffExists) return BadRequest("指定的员工ID不存在");
            }
            
            plan.ELDERLY_ID = dto.ElderlyId;
            plan.STAFF_ID = dto.StaffId;
            plan.PLAN_START_DATE = dto.PlanStartDate;
            plan.PLAN_END_DATE = dto.PlanEndDate;
            plan.CARE_TYPE = dto.CareType;
            plan.PRIORITY = dto.Priority;
            plan.EVALUATION_STATUS = dto.EvaluationStatus;
            
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("nursing-plans/{id}")]
        public async Task<IActionResult> DeleteNursingPlan(decimal id)
        {
            var plan = await _context.NursingPlans.FindAsync(id);
            if (plan == null) return NotFound();
            
            _context.NursingPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        

        // 1.1.18 消毒记录管理
        [HttpPost("disinfection/record")]
        public async Task<IActionResult> RecordDisinfection([FromBody] DisinfectionRecordDto dto)
        {
            try
            {
                await _disinfectionService.RecordDisinfectionAsync(dto);
                return Ok(new { Message = "消毒记录已保存" });
            }
            catch (Exception ex)
            {
                return BadRequest($"保存消毒记录失败: {ex.Message}");
            }
        }

        [HttpPost("disinfection/report")]
        public async Task<IActionResult> GenerateDisinfectionReport([FromBody] DisinfectionReportRequestDto dto)
        {
            try
            {
                var report = await _disinfectionService.GenerateMonthlyReportAsync(dto);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"生成报告失败: {ex.Message}");
            }
        }
    }
    // 以下是之前实现的核心业务逻辑
    // 1.1.12 紧急SOS创建与响应
    // 在控制器中使用
    [ApiController]
    [Route("api/[controller]")]
    public class EmergencySOSController : ControllerBase
    {
        private readonly IEmergencySOSService _sosService;

        public EmergencySOSController(IEmergencySOSService sosService)
        {
            _sosService = sosService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSOS([FromBody] EmergencySOSCreateDto dto)
        {
            try
            {
                var callId = await _sosService.CreateSOSRecordAsync(dto);
                return Ok(new { CallId = callId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptSOS([FromBody] EmergencySOSAcceptDto dto)
        {
            var result = await _sosService.AcceptSOSAsync(dto);
            return result ? Ok() : BadRequest("无法接受该SOS呼叫");
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteSOS([FromBody] EmergencySOSCompleteDto dto)
        {
            var result = await _sosService.CompleteSOSAsync(dto);
            return result ? Ok() : BadRequest("无法完成该SOS呼叫");
        }

        //[HttpGet("notifications/{staffId}")]
        //public async Task<IActionResult> GetNotifications(decimal staffId)
        //{
           // var notifications = await _sosService.GetActiveSOSNotificationsAsync(staffId);
           // return Ok(notifications);
       // }
    }
}