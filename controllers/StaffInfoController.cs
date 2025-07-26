using ElderlyCare.Models;
using ElderlyCare.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffInfoController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IOperationLogService _operationLogService;

        public StaffInfoController(IStaffService staffService, IOperationLogService operationLogService)
        {
            _staffService = staffService;
            _operationLogService = operationLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffInfo>>> GetAllStaff()
        {
            var staffList = await _staffService.GetAllStaffAsync();
            await LogOperation("获取所有员工信息");
            return Ok(staffList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StaffInfo>> GetStaffById(int id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            await LogOperation($"获取员工ID={id}的信息");
            return Ok(staff);
        }

        [HttpPost]
        public async Task<ActionResult<StaffInfo>> AddStaff(StaffInfo staff)
        {
            await _staffService.AddStaffAsync(staff);
            await LogOperation($"添加新员工: {staff.Name}");
            return CreatedAtAction(nameof(GetStaffById), new { id = staff.StaffId }, staff);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(int id, StaffInfo staff)
        {
            if (id != staff.StaffId)
            {
                return BadRequest();
            }

            await _staffService.UpdateStaffAsync(staff);
            await LogOperation($"更新员工ID={id}的信息");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            await _staffService.DeleteStaffAsync(id);
            await LogOperation($"删除员工ID={id}");
            return NoContent();
        }

        [HttpGet("by-position/{position}")]
        public async Task<ActionResult<IEnumerable<StaffInfo>>> GetStaffByPosition(string position)
        {
            var staffList = await _staffService.GetStaffByPositionAsync(position);
            await LogOperation($"按职位查询员工: {position}");
            return Ok(staffList);
        }

        private async Task LogOperation(string description)
        {
            await _operationLogService.LogOperationAsync(new OperationLog
            {
                OperationTime = DateTime.Now,
                OperationType = "StaffInfo",
                OperationDescription = description,
                OperationStatus = "Success",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeviceType = HttpContext.Request.Headers["User-Agent"].ToString()
            });
        }
    }
}