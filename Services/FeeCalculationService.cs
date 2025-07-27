using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Models.Dtos;

namespace api.Services;

public class FeeCalculationService
{
    private readonly AppDbContext _context;

    public FeeCalculationService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取指定老人在指定周期内的费用明细
    /// </summary>
    /// <param name="elderlyId">老人ID</param>
    /// <param name="startDate">周期开始日期</param>
    /// <param name="endDate">周期结束日期</param>
    /// <returns>费用明细列表</returns>
    public async Task<List<FeeDetailDto>> GetFeeDetailsForElderly(int elderlyId, DateTime startDate, DateTime endDate)
    {
        var details = new List<FeeDetailDto>();

        // 获取护理计划费用明细
        var nursingPlans = await _context.NursingPlans
            .Where(p => p.elderly_id == elderlyId &&
                        p.plan_start_date >= startDate &&
                        p.plan_end_date <= endDate)
            .ToListAsync();

        foreach (var plan in nursingPlans)
        {
            decimal dailyRate = plan.care_type switch
            {
                "基础护理" => 50,
                "中级护理" => 80,
                "高级护理" => 120,
                _ => 60
            };
            int days = (plan.plan_end_date - plan.plan_start_date).Days + 1;
            decimal amount = dailyRate * days;

            details.Add(new FeeDetailDto
            {
                fee_type = "护理服务",
                description = $"{plan.care_type} ({plan.plan_start_date:yyyy-MM-dd} 至 {plan.plan_end_date:yyyy-MM-dd})",
                amount = amount,
                start_date = plan.plan_start_date,
                end_date = plan.plan_end_date,
                quantity = days,
                unit_price = dailyRate
            });
        }

        // 获取药品费用明细
        var medicalOrders = await _context.MedicalOrders
            .Where(o => o.elderly_id == elderlyId &&
                        o.order_date >= startDate &&
                        o.order_date <= endDate &&
                        o.status == "已领用")
            .ToListAsync();

        foreach (var order in medicalOrders)
        {
            details.Add(new FeeDetailDto
            {
                fee_type = "药品",
                description = order.medicine_name,
                amount = order.quantity * order.unit_price,
                start_date = order.order_date,
                quantity = order.quantity,
                unit_price = order.unit_price
            });
        }

        // 获取住宿费用明细
        var roomManagement = await _context.RoomManagements
            .Where(r => r.elderly_id == elderlyId &&
                        r.check_in_date <= endDate &&
                        (r.check_out_date == null || r.check_out_date >= startDate))
            .FirstOrDefaultAsync();

        if (roomManagement != null)
        {
            var actualStartDate = roomManagement.check_in_date > startDate ? roomManagement.check_in_date : startDate;
            var actualEndDate = roomManagement.check_out_date.HasValue && roomManagement.check_out_date < endDate ? roomManagement.check_out_date.Value : endDate;
            int days = (actualEndDate - actualStartDate).Days + 1;
            decimal amount = roomManagement.daily_rate * days;

            details.Add(new FeeDetailDto
            {
                fee_type = "住宿",
                description = $"住宿费用 ({actualStartDate:yyyy-MM-dd} 至 {actualEndDate:yyyy-MM-dd})",
                amount = amount,
                start_date = actualStartDate,
                end_date = actualEndDate,
                quantity = days,
                unit_price = roomManagement.daily_rate
            });
        }

        // 获取活动费用明细
        var activities = await _context.ElderlyActivities
            .Where(ea => ea.elderly_id == elderlyId)
            .Include(ea => ea.ActivitySchedule)
            .Where(ea => ea.ActivitySchedule.activity_date >= startDate &&
                        ea.ActivitySchedule.activity_date <= endDate &&
                        ea.ActivitySchedule.is_chargeable)
            .ToListAsync();

        foreach (var activity in activities)
        {
            details.Add(new FeeDetailDto
            {
                fee_type = "活动",
                description = activity.ActivitySchedule.activity_name,
                amount = activity.ActivitySchedule.fee,
                start_date = activity.ActivitySchedule.activity_date,
                quantity = 1,
                unit_price = activity.ActivitySchedule.fee
            });
        }

        return details;
    }

    /// <summary>
        /// 为所有老人生成指定周期的费用账单
        /// </summary>
        /// <param name="startDate">周期开始日期</param>
        /// <param name="endDate">周期结束日期</param>
        /// <returns>生成的账单数量</returns>
        public async Task<int> GenerateBillsForPeriod(DateTime startDate, DateTime endDate)
        {
            // 获取所有老人
            var elderlyList = await _context.ElderlyInfos.ToListAsync();
            int generatedCount = 0;

            foreach (var elderly in elderlyList)
            {
                // 获取详细费用明细
                var feeDetails = await GetFeeDetailsForElderly(elderly.elderly_id, startDate, endDate);
                var totalAmount = feeDetails.Sum(d => d.amount);
                decimal insuranceAmount = totalAmount * 0.7m;
                decimal personalPayment = totalAmount - insuranceAmount;

                // 创建费用结算记录
                var settlement = new FeeSettlement
                {
                    elderly_id = elderly.elderly_id,
                    billing_cycle_start = startDate,
                    billing_cycle_end = endDate,
                    total_amount = totalAmount,
                    insurance_amount = insuranceAmount,
                    personal_payment = personalPayment,
                    payment_status = "待支付",
                    created_at = DateTime.Now
                };

                _context.FeeSettlements.Add(settlement);
                await _context.SaveChangesAsync();

                // 创建费用明细记录
                foreach (var detail in feeDetails)
                {
                    _context.FeeDetails.Add(new FeeDetail
                    {
                        fee_settlement_id = settlement.settlement_id,
                        fee_type = detail.fee_type,
                        description = detail.description,
                        amount = detail.amount,
                        start_date = detail.start_date,
                        end_date = detail.end_date,
                        quantity = detail.quantity,
                        unit_price = detail.unit_price
                    });
                }

                await _context.SaveChangesAsync();
                generatedCount++;
            }

            return generatedCount;
        }

    /// <summary>
    /// 计算指定老人在指定周期内的总费用
    /// </summary>
    /// <param name="elderlyId">老人ID</param>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <returns>总费用</returns>
    public async Task<decimal> CalculateTotalFee(int elderlyId, DateTime startDate, DateTime endDate)
    {
        // 获取该老人在周期内的所有护理计划
        var nursingPlans = await _context.NursingPlans
            .Where(p => p.elderly_id == elderlyId &&
                        p.plan_start_date >= startDate &&
                        p.plan_end_date <= endDate)
            .ToListAsync();

        decimal totalFee = 0;

        // 假设不同护理类型的费用标准
        foreach (var plan in nursingPlans)
        {
            switch (plan.care_type)
            {
                case "基础护理":
                    totalFee += 50 * (plan.plan_end_date.Day - plan.plan_start_date.Day + 1);
                    break;
                case "中级护理":
                    totalFee += 80 * (plan.plan_end_date.Day - plan.plan_start_date.Day + 1);
                    break;
                case "高级护理":
                    totalFee += 120 * (plan.plan_end_date.Day - plan.plan_start_date.Day + 1);
                    break;
                default:
                    totalFee += 60 * (plan.plan_end_date.Day - plan.plan_start_date.Day + 1);
                    break;
            }
        }

        // 计算药品费用
        var medicalOrders = await _context.MedicalOrders
            .Where(o => o.elderly_id == elderlyId &&
                        o.order_date >= startDate &&
                        o.order_date <= endDate &&
                        o.status == "已领用")
            .ToListAsync();

        foreach (var order in medicalOrders)
        {
            totalFee += order.quantity * order.unit_price;
        }

        // 计算住宿费用
        var roomManagement = await _context.RoomManagements
            .Where(r => r.elderly_id == elderlyId &&
                        r.check_in_date <= endDate &&
                        (r.check_out_date == null || r.check_out_date >= startDate))
            .FirstOrDefaultAsync();

        if (roomManagement != null)
        {
            // 计算周期内的住宿天数
            var actualStartDate = roomManagement.check_in_date > startDate ? roomManagement.check_in_date : startDate;
            var actualEndDate = roomManagement.check_out_date.HasValue && roomManagement.check_out_date < endDate ? roomManagement.check_out_date.Value : endDate;
            int days = actualEndDate.Day - actualStartDate.Day + 1;
            totalFee += roomManagement.daily_rate * days;
        }

        // 计算活动费用
        var activities = await _context.ElderlyActivities
            .Where(ea => ea.elderly_id == elderlyId)
            .Include(ea => ea.ActivitySchedule)
            .Where(ea => ea.ActivitySchedule.activity_date >= startDate &&
                        ea.ActivitySchedule.activity_date <= endDate &&
                        ea.ActivitySchedule.is_chargeable)
            .ToListAsync();

        foreach (var activity in activities)
        {
            totalFee += activity.ActivitySchedule.fee;
        }

        return totalFee;
    }
}
