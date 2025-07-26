using ElderlyCare.Data;
using ElderlyCare.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public class EmergencySOSService : IEmergencySOSService
    {
        private readonly ElderlyCareDbContext _context;
        private readonly INotificationService _notificationService;

        public EmergencySOSService(ElderlyCareDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<EmergencySOS> CreateEmergencyCallAsync(EmergencySOS emergencyCall)
        {
            emergencyCall.CallTime = DateTime.Now;
            emergencyCall.CallStatus = "待响应";

            _context.EmergencySOS.Add(emergencyCall);
            await _context.SaveChangesAsync();

            // 通知最近的护理人员
            await NotifyNearestStaffAsync(emergencyCall);

            return emergencyCall;
        }

        public async Task RespondToEmergencyCallAsync(int callId, int staffId)
        {
            var emergencyCall = await _context.EmergencySOS.FindAsync(callId);
            if (emergencyCall != null && emergencyCall.CallStatus == "待响应")
            {
                emergencyCall.ResponseStaffId = staffId;
                emergencyCall.ResponseTime = DateTime.Now;
                emergencyCall.CallStatus = "处理中";
                await _context.SaveChangesAsync();
            }
        }

        public async Task CompleteEmergencyCallAsync(int callId, string handlingResult)
        {
            var emergencyCall = await _context.EmergencySOS.FindAsync(callId);
            if (emergencyCall != null && emergencyCall.CallStatus == "处理中")
            {
                emergencyCall.HandlingResult = handlingResult;
                emergencyCall.CallStatus = "已完成";
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<EmergencySOS>> GetPendingEmergencyCallsAsync()
        {
            return await _context.EmergencySOS
                .Where(es => es.CallStatus == "待响应")
                //.Include(es => es.Elderly)
                //.Include(es => es.Room)
                .ToListAsync();
        }

        public async Task<List<EmergencySOS>> GetEmergencyCallsByStaffIdAsync(int staffId)
        {
            return await _context.EmergencySOS
                .Where(es => es.ResponseStaffId == staffId)
                //.Include(es => es.Elderly)
                //.Include(es => es.Room)
                .ToListAsync();
        }

        public async Task<List<EmergencySOS>> GetEmergencyCallsByElderlyIdAsync(int elderlyId)
        {
            return await _context.EmergencySOS
                .Where(es => es.ElderlyId == elderlyId)
                .Include(es => es.ResponseStaff)
                //.Include(es => es.Room)
                .ToListAsync();
        }

        private async Task NotifyNearestStaffAsync(EmergencySOS emergencyCall)
        {
            // 获取所有在岗的护理人员
            var onDutyStaff = await _context.StaffInfos
                .Where(s => s.Position == "护理人员")
                .ToListAsync();

            // 这里应该有逻辑计算最近的护理人员
            // 简化示例：随机选择一个护理人员
            if (onDutyStaff.Any())
            {
                var randomStaff = onDutyStaff.OrderBy(x => Guid.NewGuid()).First();
                
                // 发送通知
                //await _notificationService.SendNotificationAsync(
                    //randomStaff.StaffId,
                    //"紧急呼叫通知",
                    //$"老人{emergencyCall.Elderly?.Name} 在 {emergencyCall.Room?.RoomNumber} 房间触发了紧急呼叫");
            }
        }
    }
}
