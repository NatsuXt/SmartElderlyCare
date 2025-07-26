using ElderlyCare.Models;
using ElderlyCare.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCare.Controllers
{
    [Route("api/nursing-plans")]
    [ApiController]
    public class NursingPlanController : ControllerBase
    {
        private readonly INursingPlanService _nursingPlanService;
        private readonly INursingSchedulerService _nursingSchedulerService;
        private readonly IOperationLogService _operationLogService;

        public NursingPlanController(
            INursingPlanService nursingPlanService,
            INursingSchedulerService nursingSchedulerService,
            IOperationLogService operationLogService)
        {
            _nursingPlanService = nursingPlanService;
            _nursingSchedulerService = nursingSchedulerService;
            _operationLogService = operationLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NursingPlan>>> GetAllNursingPlans()
        {
            var plans = await _nursingPlanService.GetAllNursingPlansAsync();
            await LogOperation("获取所有护理计划");
            return Ok(plans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NursingPlan>> GetNursingPlanById(int id)
        {
            var plan = await _nursingPlanService.GetNursingPlanByIdAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            await LogOperation($"获取护理计划ID={id}");
            return Ok(plan);
        }

        [HttpPost]
        public async Task<ActionResult<NursingPlan>> CreateNursingPlan(NursingPlan plan)
        {
            await _nursingPlanService.CreateNursingPlanAsync(plan);
            await LogOperation($"创建新护理计划ID={plan.PlanId}");
            return CreatedAtAction(nameof(GetNursingPlanById), new { id = plan.PlanId }, plan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNursingPlan(int id, NursingPlan plan)
        {
            if (id != plan.PlanId)
            {
                return BadRequest();
            }

            await _nursingPlanService.UpdateNursingPlanAsync(plan);
            await LogOperation($"更新护理计划ID={id}");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNursingPlan(int id)
        {
            await _nursingPlanService.DeleteNursingPlanAsync(id);
            await LogOperation($"删除护理计划ID={id}");
            return NoContent();
        }

        [HttpGet("unassigned")]
        public async Task<ActionResult<IEnumerable<NursingPlan>>> GetUnassignedNursingPlans()
        {
            var plans = await _nursingPlanService.GetUnassignedNursingPlansAsync();
            await LogOperation("获取未分配的护理计划");
            return Ok(plans);
        }

        [HttpGet("by-staff/{staffId}")]
        public async Task<ActionResult<IEnumerable<NursingPlan>>> GetNursingPlansByStaffId(int staffId)
        {
            var plans = await _nursingPlanService.GetNursingPlansByStaffIdAsync(staffId);
            await LogOperation($"获取员工ID={staffId}的护理计划");
            return Ok(plans);
        }

        [HttpPost("assign/{planId}/{staffId}")]
        public async Task<IActionResult> AssignNursingPlan(int planId, int staffId)
        {
            await _nursingPlanService.AssignNursingPlanAsync(planId, staffId);
            await LogOperation($"分配护理计划ID={planId}给员工ID={staffId}");
            return NoContent();
        }

        [HttpPost("auto-assign")]
        public async Task<IActionResult> AutoAssignNursingPlans()
        {
            await _nursingSchedulerService.AutoAssignNursingPlansAsync();
            await LogOperation("自动分配护理计划");
            return NoContent();
        }

        private async Task LogOperation(string description)
        {
            await _operationLogService.LogOperationAsync(new OperationLog
            {
                OperationTime = DateTime.Now,
                OperationType = "NursingPlan",
                OperationDescription = description,
                OperationStatus = "Success",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeviceType = HttpContext.Request.Headers["User-Agent"].ToString()
            });
        }
    }
}