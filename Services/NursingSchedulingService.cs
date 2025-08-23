using Staff_Info.Data;
using Staff_Info.DTOs;
using Staff_Info.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Staff_Info.Services
{
    public interface INursingSchedulingService
    {
        Task<string> GenerateNursingScheduleAsync();
    }

    public class NursingSchedulingService : INursingSchedulingService
    {
        private readonly StaffInfoDbContext _context;
        private readonly Action<string> _log;

        public NursingSchedulingService(StaffInfoDbContext context, Action<string> log = null)
        {
            _context = context;
            _log = log ?? Console.WriteLine;
        }

        public async Task<string> GenerateNursingScheduleAsync()
        {
            try
            {
                _log("=== 开始排班流程 ===");
                
                // 步骤1: 先获取原始护理人员数据
                _log("获取护理人员原始数据...");
                var rawStaffData = await _context.StaffInfos
                    .Where(s => s.POSITION == "Nurse") 
                    .Select(s => new {
                        s.STAFF_ID,
                        s.SKILL_LEVEL,
                        AssignmentCount = s.NURSINGPLANS.Count(np => np.EVALUATION_STATUS != "Completed")
                    })
                    .AsNoTracking()
                    .ToListAsync();

                // 在内存中进行标准化处理
                var staffList = rawStaffData.Select(s => new {
                    s.STAFF_ID,
                    OriginalSkillLevel = s.SKILL_LEVEL,
                    SKILL_LEVEL = NormalizeSkillLevel(s.SKILL_LEVEL),
                    CurrentAssignments = s.AssignmentCount
                }).OrderByDescending(s => s.SKILL_LEVEL).ToList();

                _log($"获取到 {staffList.Count} 名护理人员");
                _log("护理人员技能等级分布:");
                staffList.GroupBy(s => s.SKILL_LEVEL)
                    .ToList()
                    .ForEach(g => _log($"  {g.Key}: {g.Count()}人"));

                // 步骤2: 获取原始护理计划数据
                _log("\n获取待处理护理计划原始数据...");
                var rawPlanData = await _context.NursingPlans
                    .Where(np => np.EVALUATION_STATUS == "Pending" && np.STAFF_ID == null)
                    .Select(np => new {
                        np.PLAN_ID,
                        np.ELDERLY_ID,
                        np.CARE_TYPE,
                        np.PRIORITY
                    })
                    .AsNoTracking()
                    .ToListAsync();

                // 在内存中进行标准化和排序
                var nursingPlans = rawPlanData.Select(np => new {
                    np.PLAN_ID,
                    np.ELDERLY_ID,
                    OriginalCareType = np.CARE_TYPE,
                    CARE_TYPE = NormalizeCareType(np.CARE_TYPE),
                    OriginalPriority = np.PRIORITY,
                    PRIORITY = NormalizePriority(np.PRIORITY)
                })
                .OrderByDescending(np => np.PRIORITY)
                .ThenByDescending(np => np.CARE_TYPE)
                .ToList();

                _log($"获取到 {nursingPlans.Count} 个待处理护理计划");
                _log("护理计划类型分布:");
                nursingPlans.GroupBy(p => p.CARE_TYPE)
                    .ToList()
                    .ForEach(g => _log($"  {g.Key}: {g.Count()}个"));
                _log("护理计划优先级分布:");
                nursingPlans.GroupBy(p => p.PRIORITY)
                    .ToList()
                    .ForEach(g => _log($"  {g.Key}: {g.Count()}个"));

                // 步骤3: 排班算法核心逻辑
                _log("\n开始分配护理计划...");
                var assignmentPlan = new List<(decimal PlanId, decimal StaffId)>();
                var unassignedPlans = new List<decimal>();

                foreach (var plan in nursingPlans)
                {
                    _log($"\n处理计划 ID: {plan.PLAN_ID}, 类型: {plan.CARE_TYPE}, 优先级: {plan.PRIORITY}");
                    _log($"原始数据 - 类型: {plan.OriginalCareType}, 优先级: {plan.OriginalPriority}");

                    var requiredSkillLevel = GetRequiredSkillLevel(plan.CARE_TYPE);
                    _log($"所需技能等级: {requiredSkillLevel}");

                    var qualifiedStaff = staffList
                        .Where(s => IsStaffQualified(s.SKILL_LEVEL, requiredSkillLevel))
                        .OrderBy(s => s.CurrentAssignments)
                        .ToList();

                    _log($"符合条件的护理人员: {qualifiedStaff.Count}人");
                    if (qualifiedStaff.Any())
                    {
                        var assignedStaff = qualifiedStaff.First();
                        _log($"分配给员工 ID: {assignedStaff.STAFF_ID}, 技能: {assignedStaff.SKILL_LEVEL}, 当前任务数: {assignedStaff.CurrentAssignments}");
                        
                        assignmentPlan.Add((plan.PLAN_ID, assignedStaff.STAFF_ID));
                        
                        // 更新内存中的任务计数
                        var staff = staffList.First(s => s.STAFF_ID == assignedStaff.STAFF_ID);
                        staffList.Remove(staff);
                        staffList.Add(new {
                            staff.STAFF_ID,
                            staff.OriginalSkillLevel,
                            staff.SKILL_LEVEL,
                            CurrentAssignments = staff.CurrentAssignments + 1
                        });
                    }
                    else
                    {
                        _log("⚠️ 没有符合条件的护理人员");
                        unassignedPlans.Add(plan.PLAN_ID);
                    }
                }

                _log($"\n分配结果: 成功分配 {assignmentPlan.Count} 个, 未分配 {unassignedPlans.Count} 个");

                // 步骤4: 更新护理计划表
                _log("\n开始更新数据库...");
                int updatedCount = 0;

                foreach (var assignment in assignmentPlan)
                {
                    var plan = await _context.NursingPlans
                        .Where(np => np.PLAN_ID == assignment.PlanId)
                        .FirstOrDefaultAsync();
                    if (plan != null)
                    {
                        _log($"更新计划 ID: {plan.PLAN_ID}, 分配员工 ID: {assignment.StaffId}");
                        plan.STAFF_ID = assignment.StaffId;
                        plan.EVALUATION_STATUS = "Scheduled";
                        _context.Entry(plan).State = EntityState.Modified;
                        updatedCount++;
                    }
                    else
                    {
                        _log($"❌ 未找到计划 ID: {assignment.PlanId}");
                    }
                }

                _log($"尝试保存 {updatedCount} 条更新...");
                var saveResult = await _context.SaveChangesAsync();
                _log($"数据库返回 {saveResult} 条记录受影响");

                // 验证更新结果
                _log("\n验证更新结果...");
                var successCount = await _context.NursingPlans
                    .CountAsync(np => assignmentPlan.Select(a => a.PlanId).Contains(np.PLAN_ID) && 
                                     np.EVALUATION_STATUS == "Scheduled" && 
                                     np.STAFF_ID != null);
                
                _log($"验证通过: {successCount} 条记录已成功更新");

                if (unassignedPlans.Count > 0)
                {
                    _log($"未分配的计划 ID: {string.Join(", ", unassignedPlans)}");
                }

                _log("=== 排班流程结束 ===");

                return unassignedPlans.Count > 0 
                    ? $"排班完成，成功分配{assignmentPlan.Count}个任务，{unassignedPlans.Count}个任务因资源不足未分配" 
                    : "所有任务已成功分配";
            }
            catch (Exception ex)
            {
                _log($"❌ 排班失败: {ex}");
                return $"排班失败: {ex.Message}";
            }
        }

        private static bool IsStaffQualified(string staffSkillLevel, string requiredSkillLevel)
        {
            // 技能等级映射到数值
            var skillLevels = new Dictionary<string, int>
            {
                {"Basic", 1}, {"基础", 1},
                {"Intermediate", 2}, {"中级", 2},
                {"Advanced", 3}, {"高级", 3}
            };

            if (!skillLevels.TryGetValue(staffSkillLevel, out var staffLevel) ||
                !skillLevels.TryGetValue(requiredSkillLevel, out var requiredLevel))
            {
                return false;
            }

            return staffLevel >= requiredLevel;
        }

        private static string GetRequiredSkillLevel(string careType)
        {
            return careType switch
            {
                "Emergency" or "紧急" => "Advanced",
                "Normal" or "普通" => "Intermediate",
                _ => "Basic" // 默认Basic
            };
        }

        private static string NormalizeSkillLevel(string skillLevel)
        {
            return skillLevel switch
            {
                "高级" => "Advanced",
                "中级" => "Intermediate",
                "基础" => "Basic",
                _ => skillLevel // 保持原样
            };
        }

        private static string NormalizeCareType(string careType)
        {
            return careType switch
            {
                "紧急" => "Emergency",
                "普通" => "Normal",
                "基础" => "Basic",
                _ => careType // 保持原样
            };
        }

        private static string NormalizePriority(string priority)
        {
            return priority switch
            {
                "高" => "High",
                "中" => "Medium",
                "低" => "Low",
                _ => priority // 保持原样
            };
        }
    }
}