// Services/EmergencySosService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElderlyCareManagement.Data;
using ElderlyCareManagement.DTOs;
using ElderlyCareManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ElderlyCareManagement.Services
{
    public interface IEmergencySosService
    {
        Task<EmergencySosDTO> CreateEmergencySosAsync(EmergencySosCreateDTO sosDto);
        Task<List<StaffInfoDTO>> FindAvailableRespondersAsync(int? roomId);
        Task AssignResponderToSosAsync(int callId, int staffId);
        Task CompleteSosCallAsync(EmergencySosUpdateDTO sosUpdateDto);
        Task<List<EmergencySosDTO>> GetPendingSosCallsAsync();
        Task<EmergencySosDetailDTO> GetSosCallDetailAsync(int callId);
    }

    public class EmergencySosService : IEmergencySosService
    {
        private readonly ElderlyCareContext _context;
        private readonly INotificationService _notificationService;

        public EmergencySosService(ElderlyCareContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<EmergencySosDTO> CreateEmergencySosAsync(EmergencySosCreateDTO sosDto)
        {
            var sosCall = new EmergencySOS
            {
                ElderlyId = sosDto.ElderlyId,
                CallTime = DateTime.Now,
                CallType = sosDto.CallType,
                RoomId = sosDto.RoomId,
                CallStatus = "待响应"
            };

            _context.EmergencySos.Add(sosCall);
            await _context.SaveChangesAsync();

            // 获取最近的3名护理人员
            var responders = (await FindAvailableRespondersAsync(sosDto.RoomId))
                .Take(3)
                .ToList();

            // 发送通知并记录通知时间
            foreach (var responder in responders)
            {
                await _notificationService.SendSosNotificationAsync(responder.StaffId, sosCall.CallId);
                
                // 记录通知发送情况
                var notification = new SosNotification
                {
                    CallId = sosCall.CallId,
                    StaffId = responder.StaffId,
                    NotificationTime = DateTime.Now,
                    IsResponded = false
                };
                _context.SosNotifications.Add(notification);
            }
            await _context.SaveChangesAsync();

            return new EmergencySosDTO
            {
                CallId = sosCall.CallId,
                ElderlyId = sosCall.ElderlyId,
                CallTime = sosCall.CallTime,
                CallType = sosCall.CallType,
                CallStatus = sosCall.CallStatus
            };
        }

        public async Task<List<StaffInfoDTO>> FindAvailableRespondersAsync(int? roomId)
        {
            // 获取当前在岗的护理人员（排班表中今天有班次且当前时间在班次内）
            var currentTime = DateTime.Now.TimeOfDay;
            var today = DateTime.Today.DayOfWeek;
            
            var availableStaff = await _context.StaffInfos
                .Where(s => s.Position == "护理人员")
                .Join(_context.StaffSchedules,
                    staff => staff.StaffId,
                    schedule => schedule.StaffId,
                    (staff, schedule) => new { staff, schedule })
                .Where(x => x.schedule.DayOfWeek == today &&
                           x.schedule.StartTime <= currentTime &&
                           x.schedule.EndTime >= currentTime)
                .Select(x => new StaffInfoDTO
                {
                    StaffId = x.staff.StaffId,
                    Name = x.staff.Name,
                    Gender = x.staff.Gender,
                    Position = x.staff.Position,
                    ContactPhone = x.staff.ContactPhone,
                    Email = x.staff.Email,
                    HireDate = x.staff.HireDate,
                    SkillLevel = x.staff.SkillLevel,
                    // 获取员工当前位置（从StaffLocation表）
                    CurrentRoomId = _context.StaffLocations
                        .Where(l => l.StaffId == x.staff.StaffId)
                        .OrderByDescending(l => l.UpdateTime)
                        .Select(l => l.RoomId)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // 如果有房间信息，优先选择距离最近的护理人员
            if (roomId.HasValue)
            {
                // 计算每个护理人员到呼叫房间的距离（简单按楼层和房间号计算）
                foreach (var staff in availableStaff)
                {
                    if (staff.CurrentRoomId.HasValue)
                    {
                        var staffRoom = await _context.Rooms.FindAsync(staff.CurrentRoomId);
                        var sosRoom = await _context.Rooms.FindAsync(roomId);
                        
                        if (staffRoom != null && sosRoom != null)
                        {
                            // 简单距离计算：不同楼层距离+100，同楼层按房间号差值
                            staff.Distance = staffRoom.Floor == sosRoom.Floor 
                                ? Math.Abs(staffRoom.RoomNumber - sosRoom.RoomNumber)
                                : Math.Abs(staffRoom.Floor - sosRoom.Floor) * 100 + 50;
                        }
                    }
                    else
                    {
                        staff.Distance = int.MaxValue; // 没有位置信息的排在最后
                    }
                }

                // 按距离排序
                return availableStaff.OrderBy(s => s.Distance).ToList();
            }

            return availableStaff;
        }

        public async Task AssignResponderToSosAsync(int callId, int staffId)
        {
            // 检查该员工是否收到了通知
            var notification = await _context.SosNotifications
                .FirstOrDefaultAsync(n => n.CallId == callId && n.StaffId == staffId);
            
            if (notification == null)
            {
                throw new InvalidOperationException("该员工未收到此SOS呼叫的通知");
            }

            var sosCall = await _context.EmergencySos.FindAsync(callId);
            if (sosCall != null && sosCall.CallStatus == "待响应")
            {
                sosCall.ResponseStaffId = staffId;
                sosCall.ResponseTime = DateTime.Now;
                sosCall.CallStatus = "处理中";
                
                // 标记通知为已响应
                notification.IsResponded = true;
                notification.ResponseTime = DateTime.Now;
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task CompleteSosCallAsync(EmergencySosUpdateDTO sosUpdateDto)
        {
            var sosCall = await _context.EmergencySos.FindAsync(sosUpdateDto.CallId);
            if (sosCall != null && sosCall.CallStatus == "处理中")
            {
                sosCall.HandlingResult = sosUpdateDto.HandlingResult;
                sosCall.CallStatus = sosUpdateDto.CallStatus;
                sosCall.FollowUpRequired = sosUpdateDto.HandlingResult.Contains("需要跟进");
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<EmergencySosDTO>> GetPendingSosCallsAsync()
        {
            return await _context.EmergencySos
                .Where(s => s.CallStatus != "已完成")
                .Select(s => new EmergencySosDTO
                {
                    CallId = s.CallId,
                    ElderlyId = s.ElderlyId,
                    CallTime = s.CallTime,
                    CallType = s.CallType,
                    CallStatus = s.CallStatus
                })
                .ToListAsync();
        }

        public async Task<EmergencySosDetailDTO> GetSosCallDetailAsync(int callId)
        {
            return await _context.EmergencySos
                .Where(s => s.CallId == callId)
                .Select(s => new EmergencySosDetailDTO
                {
                    CallId = s.CallId,
                    ElderlyId = s.ElderlyId,
                    CallTime = s.CallTime,
                    CallType = s.CallType,
                    CallStatus = s.CallStatus,
                    RoomId = s.RoomId,
                    ResponseTime = s.ResponseTime,
                    ResponseStaffId = s.ResponseStaffId,
                    ResponseStaffName = s.ResponseStaff.Name,
                    FollowUpRequired = s.FollowUpRequired,
                    HandlingResult = s.HandlingResult
                })
                .FirstOrDefaultAsync();
        }
    }
}