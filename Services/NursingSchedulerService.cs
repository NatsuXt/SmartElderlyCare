using ElderlyCare.Data;
using ElderlyCare.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public class NursingSchedulerService : INursingSchedulerService
    {
        private readonly ElderlyCareDbContext _context;
        private readonly INotificationService _notificationService;

        public NursingSchedulerService(ElderlyCareDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task AutoAssignNursingPlansAsync()
        {
            // 1. 获取所有护理人员的工作时间和技能等级
            var staffList = await _context.StaffInfos
                .Where(s => s.Position == "护理人员")
                .ToListAsync();

            // 2. 获取所有未分配的护理需求
            var unassignedPlans = await _context.NursingPlans
                .Where(np => np.StaffId == null)
                //.Include(np => np.Elderly)
                .ToListAsync();

            // 3. 排班算法
            foreach (var plan in unassignedPlans)
            {
                // 简化示例：根据护理类型匹配技能
                var suitableStaff = staffList
                    .Where(s => IsStaffSuitableForPlan(s, plan))
                    .OrderBy(s => GetCurrentWorkload(s.StaffId))
                    .FirstOrDefault();

                if (suitableStaff != null)
                {
                    plan.StaffId = suitableStaff.StaffId;
                    await _context.SaveChangesAsync();

                    // 发送通知给护理人员
                    // await _notificationService.SendNotificationAsync(
                    // suitableStaff.StaffId,
                    // "新护理任务分配",
                    //  $"您已被分配到为 {plan.Elderly?.Name} 执行 {plan.CareType} 护理任务");
                }
            }
        }

        private bool IsStaffSuitableForPlan(StaffInfo staff, NursingPlan plan)
        {
            // 简化示例：实际项目中应该有更复杂的逻辑
            // 例如检查员工技能是否匹配护理需求
            return true;
        }

        private int GetCurrentWorkload(int staffId)
        {
            // 简化示例：获取员工当前的护理任务数量
            return _context.NursingPlans.Count(np => np.StaffId == staffId);
        }
    }
}