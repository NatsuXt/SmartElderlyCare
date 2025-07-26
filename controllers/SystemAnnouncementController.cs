using ElderlyCare.Models;
using ElderlyCare.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCare.Controllers
{
    [Route("api/announcements")]
    [ApiController]
    public class SystemAnnouncementController : ControllerBase
    {
        private readonly ISystemAnnouncementService _announcementService;
        private readonly IOperationLogService _operationLogService;

        public SystemAnnouncementController(
            ISystemAnnouncementService announcementService,
            IOperationLogService operationLogService)
        {
            _announcementService = announcementService;
            _operationLogService = operationLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemAnnouncement>>> GetAllAnnouncements()
        {
            var announcements = await _announcementService.GetActiveAnnouncementsAsync();
            await LogOperation("获取所有系统公告");
            return Ok(announcements);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SystemAnnouncement>> GetAnnouncementById(int id)
        {
            var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }
            await LogOperation($"获取公告ID={id}");
            return Ok(announcement);
        }

        [HttpPost]
        public async Task<ActionResult<SystemAnnouncement>> CreateAnnouncement(SystemAnnouncement announcement)
        {
            await _announcementService.CreateAnnouncementAsync(announcement);
            await LogOperation($"创建新公告ID={announcement.AnnouncementId}");
            return CreatedAtAction(nameof(GetAnnouncementById), new { id = announcement.AnnouncementId }, announcement);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnnouncement(int id, SystemAnnouncement announcement)
        {
            if (id != announcement.AnnouncementId)
            {
                return BadRequest();
            }

            await _announcementService.UpdateAnnouncementAsync(announcement);
            await LogOperation($"更新公告ID={id}");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            await _announcementService.DeleteAnnouncementAsync(id);
            await LogOperation($"删除公告ID={id}");
            return NoContent();
        }

        [HttpGet("by-type/{type}")]
        public async Task<ActionResult<IEnumerable<SystemAnnouncement>>> GetAnnouncementsByType(string type)
        {
            var announcements = await _announcementService.GetAnnouncementsByTypeAsync(type);
            await LogOperation($"按类型查询公告: {type}");
            return Ok(announcements);
        }

        [HttpGet("by-creator/{staffId}")]
        public async Task<ActionResult<IEnumerable<SystemAnnouncement>>> GetAnnouncementsByCreator(int staffId)
        {
            var announcements = await _announcementService.GetAnnouncementsByCreatorAsync(staffId);
            await LogOperation($"获取员工ID={staffId}创建的公告");
            return Ok(announcements);
        }

        private async Task LogOperation(string description)
        {
            await _operationLogService.LogOperationAsync(new OperationLog
            {
                OperationTime = DateTime.Now,
                OperationType = "SystemAnnouncement",
                OperationDescription = description,
                OperationStatus = "Success",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeviceType = HttpContext.Request.Headers["User-Agent"].ToString()
            });
        }
    }
}