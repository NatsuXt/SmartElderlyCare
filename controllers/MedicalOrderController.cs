using ElderlyCare.Models;
using ElderlyCare.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCare.Controllers
{
    [Route("api/medical-orders")]
    [ApiController]
    public class MedicalOrderController : ControllerBase
    {
        private readonly IMedicalOrderService _medicalOrderService;
        private readonly IOperationLogService _operationLogService;

        public MedicalOrderController(
            IMedicalOrderService medicalOrderService,
            IOperationLogService operationLogService)
        {
            _medicalOrderService = medicalOrderService;
            _operationLogService = operationLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalOrder>>> GetAllMedicalOrders()
        {
            var orders = await _medicalOrderService.GetAllMedicalOrdersAsync();
            await LogOperation("获取所有医嘱");
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalOrder>> GetMedicalOrderById(int id)
        {
            var order = await _medicalOrderService.GetMedicalOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            await LogOperation($"获取医嘱ID={id}");
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<MedicalOrder>> CreateMedicalOrder(MedicalOrder order)
        {
            await _medicalOrderService.CreateMedicalOrderAsync(order);
            await LogOperation($"创建新医嘱ID={order.OrderId}");
            return CreatedAtAction(nameof(GetMedicalOrderById), new { id = order.OrderId }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicalOrder(int id, MedicalOrder order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            await _medicalOrderService.UpdateMedicalOrderAsync(order);
            await LogOperation($"更新医嘱ID={id}");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalOrder(int id)
        {
            await _medicalOrderService.DeleteMedicalOrderAsync(id);
            await LogOperation($"删除医嘱ID={id}");
            return NoContent();
        }

        [HttpGet("by-staff/{staffId}")]
        public async Task<ActionResult<IEnumerable<MedicalOrder>>> GetMedicalOrdersByStaffId(int staffId)
        {
            var orders = await _medicalOrderService.GetMedicalOrdersByStaffIdAsync(staffId);
            await LogOperation($"获取员工ID={staffId}的医嘱");
            return Ok(orders);
        }

        [HttpGet("by-elderly/{elderlyId}")]
        public async Task<ActionResult<IEnumerable<MedicalOrder>>> GetMedicalOrdersByElderlyId(int elderlyId)
        {
            var orders = await _medicalOrderService.GetMedicalOrdersByElderlyIdAsync(elderlyId);
            await LogOperation($"获取老人ID={elderlyId}的医嘱");
            return Ok(orders);
        }

        private async Task LogOperation(string description)
        {
            await _operationLogService.LogOperationAsync(new OperationLog
            {
                OperationTime = DateTime.Now,
                OperationType = "MedicalOrder",
                OperationDescription = description,
                OperationStatus = "Success",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeviceType = HttpContext.Request.Headers["User-Agent"].ToString()
            });
        }
    }
}