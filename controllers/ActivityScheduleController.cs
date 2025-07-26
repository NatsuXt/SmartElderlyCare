using ElderlyCare.Models;
using ElderlyCare.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCare.Controllers
{
    [Route("api/activities")]
    [ApiController]
    public class ActivityScheduleController : ControllerBase
    {
        private readonly IActivityScheduleService _activityService;
        private readonly IOperationLogService _operationLogService;

        public ActivityScheduleController(
            IActivityScheduleService activityService,
            IOperationLogService operationLogService)
        {
            _activityService = activityService;
            _operationLogService = operationLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivitySchedule>>> GetAllActivities()
        {
            var activities = await _activityService.GetAllActivitiesAsync();
            await LogOperation("获取所有活动安排");
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivitySchedule>> GetActivityById(int id)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            await LogOperation($"获取活动ID={id}");
            return Ok(activity);
        }

        [HttpPost]
        public async Task<ActionResult<ActivitySchedule>> CreateActivity(ActivitySchedule activity)
        {
            await _activityService.CreateActivityAsync(activity);
            await LogOperation($"创建新活动ID={activity.ActivityId}");
            return CreatedAtAction(nameof(GetActivityById), new { id = activity.ActivityId }, activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(int id, ActivitySchedule activity)
        {
            if (id != activity.ActivityId)
            {
                return BadRequest();
            }

            await _activityService.UpdateActivityAsync(activity);
            await LogOperation($"更新活动ID={id}");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            await _activityService.DeleteActivityAsync(id);
            await LogOperation($"删除活动ID={id}");
            return NoContent();
        }

        [HttpGet("by-staff/{staffId}")]
        public async Task<ActionResult<IEnumerable<ActivitySchedule>>> GetActivitiesByStaffId(int staffId)
        {
            var activities = await _activityService.GetActivitiesByStaffIdAsync(staffId);
            await LogOperation($"获取员工ID={staffId}的活动安排");
            return Ok(activities);
        }

        [HttpGet("upcoming/{days}")]
        public async Task<ActionResult<IEnumerable<ActivitySchedule>>> GetUpcomingActivities(int days)
        {
            var activities = await _activityService.GetUpcomingActivitiesAsync(days);
            await LogOperation($"获取未来{days}天的活动安排");
            return Ok(activities);
        }

        private async Task LogOperation(string description)
        {
            await _operationLogService.LogOperationAsync(new OperationLog
            {
                OperationTime = DateTime.Now,
                OperationType = "ActivitySchedule",
                OperationDescription = description,
                OperationStatus = "Success",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeviceType = HttpContext.Request.Headers["User-Agent"].ToString()
            });
        }
    }
}