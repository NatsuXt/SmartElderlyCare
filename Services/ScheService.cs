// Services/NursingScheduleService.cs
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
    public interface INursingScheduleService
    {
        Task<List<NursingPlanDTO>> GetUnassignedNursingPlansAsync();
        Task<List<StaffInfoDTO>> GetAvailableStaffAsync(DateTime startTime, DateTime endTime, string requiredSkill);
        Task AssignStaffToNursingPlanAsync(int nursingPlanId, int staffId);
        Task AutoScheduleNursingPlansAsync();
        Task<NursingPlanDetailDTO> GetNursingPlanDetailAsync(int planId);
    }

    public class NursingScheduleService : INursingScheduleService
    {
        private readonly ElderlyCareContext _context;

        public NursingScheduleService(ElderlyCareContext context)
        {
            _context = context;
        }

        public async Task<List<NursingPlanDTO>> GetUnassignedNursingPlansAsync()
        {
            return await _context.NursingPlans
                .Where(np => np.StaffId == null && np.PlanEndDate >= DateTime.Today)
                .OrderByDescending(np => np.Priority) // 按优先级排序
                .ThenBy(np => np.PlanStartDate) // 然后按开始时间排序
                .Select(np => new NursingPlanDTO
                {
                    PlanId = np.PlanId,
                    ElderlyId = np.ElderlyId,
                    PlanStartDate = np.PlanStartDate,
                    PlanEndDate = np.PlanEndDate,
                    CareType = np.CareType,
                    Priority = np.Priority
                })
                .ToListAsync();
        }

        public async Task<List<StaffInfoDTO>> GetAvailableStaffAsync(DateTime startTime, DateTime endTime, string requiredSkill)
        {
            // 获取所有具备所需技能的员工
            var skilledStaff = await _context.StaffInfos
                .Where(s => s.SkillLevel.Contains(requiredSkill))
                .ToListAsync();

            // 获取这些员工在当前时间段内已有的排班
            var existingAssignments = await _context.NursingPlans
                .Where(np => np.StaffId != null && 
                            np.PlanStartDate < endTime && 
                            np.PlanEndDate > startTime)
                .ToListAsync();

            // 计算每个员工在当前时间段内的工作量
            var staffWorkloads = new Dictionary<int, int>();
            foreach (var staff in skilledStaff)
            {
                var workload = existingAssignments
                    .Count(np => np.StaffId == staff.StaffId);
                staffWorkloads.Add(staff.StaffId, workload);
            }

            // 返回可用员工，按工作量升序排序以实现均衡分配
            return skilledStaff
                .OrderBy(s => staffWorkloads[s.StaffId])
                .Select(s => new StaffInfoDTO
                {
                    StaffId = s.StaffId,
                    Name = s.Name,
                    Gender = s.Gender,
                    Position = s.Position,
                    ContactPhone = s.ContactPhone,
                    Email = s.Email,
                    HireDate = s.HireDate,
                    SkillLevel = s.SkillLevel
                })
                .ToList();
        }

        public async Task AssignStaffToNursingPlanAsync(int nursingPlanId, int staffId)
        {
            var nursingPlan = await _context.NursingPlans.FindAsync(nursingPlanId);
            if (nursingPlan != null)
            {
                nursingPlan.StaffId = staffId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AutoScheduleNursingPlansAsync()
        {
            var unassignedPlans = await GetUnassignedNursingPlansAsync();
            
            foreach (var planDto in unassignedPlans)
            {
                // 根据护理类型确定所需技能
                var requiredSkill = planDto.CareType switch
                {
                    "日常护理" => "基础护理",
                    "重症护理" => "急救护理",
                    "术后护理" => "术后护理",
                    "康复护理" => "康复训练",
                    _ => "基础护理"
                };

                // 获取可用员工（考虑时间段和技能）
                var availableStaff = await GetAvailableStaffAsync(
                    planDto.PlanStartDate, 
                    planDto.PlanEndDate, 
                    requiredSkill);
                
                if (availableStaff.Any())
                {
                    // 选择最适合的员工（考虑技能匹配度和工作负荷）
                    var selectedStaff = SelectBestStaff(availableStaff, requiredSkill);
                    await AssignStaffToNursingPlanAsync(planDto.PlanId, selectedStaff.StaffId);
                }
                else
                {
                    // 如果没有完全匹配的员工，尝试放宽技能要求
                    var fallbackSkill = requiredSkill == "急救护理" ? "基础护理" : null;
                    if (fallbackSkill != null)
                    {
                        var fallbackStaff = await GetAvailableStaffAsync(
                            planDto.PlanStartDate, 
                            planDto.PlanEndDate, 
                            fallbackSkill);
                        
                        if (fallbackStaff.Any())
                        {
                            var selectedStaff = SelectBestStaff(fallbackStaff, fallbackSkill);
                            await AssignStaffToNursingPlanAsync(planDto.PlanId, selectedStaff.StaffId);
                        }
                    }
                }
            }
        }

        private StaffInfoDTO SelectBestStaff(List<StaffInfoDTO> availableStaff, string requiredSkill)
        {
            // 优先选择技能完全匹配的员工
            var perfectMatch = availableStaff
                .FirstOrDefault(s => s.SkillLevel.Split(',').Contains(requiredSkill));
            
            if (perfectMatch != null)
            {
                return perfectMatch;
            }

            // 如果没有完全匹配的，选择技能包含所需关键词的员工
            return availableStaff
                .OrderBy(s => s.SkillLevel.IndexOf(requiredSkill, StringComparison.OrdinalIgnoreCase) >= 0 ? 0 : 1)
                .ThenBy(s => s.HireDate) // 资历较深的优先
                .First();
        }

        public async Task<NursingPlanDetailDTO> GetNursingPlanDetailAsync(int planId)
        {
            return await _context.NursingPlans
                .Where(np => np.PlanId == planId)
                .Select(np => new NursingPlanDetailDTO
                {
                    PlanId = np.PlanId,
                    ElderlyId = np.ElderlyId,
                    PlanStartDate = np.PlanStartDate,
                    PlanEndDate = np.PlanEndDate,
                    CareType = np.CareType,
                    Priority = np.Priority,
                    StaffId = np.StaffId,
                    StaffName = np.Staff.Name,
                    EvaluationStatus = np.EvaluationStatus
                })
                .FirstOrDefaultAsync();
        }
    }
}