using System;
using System.Collections.Generic;
using System.Linq;

public class VisitorRegistrationService
{
    private readonly AppDbContext _context;

    public VisitorRegistrationService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 家属提交远程视频探视预约申请。
    /// </summary>
    /// <param name="registration">预约实体，需包含 family_id、elderly_id、visit_time、visitor_name、relationship_to_elderly、visit_reason</param>
    /// <returns>新建的预约记录</returns>
    public VisitorRegistration SubmitVideoVisit(VisitorRegistration registration)
    {
        // 业务逻辑验证
        if (registration.visit_time != default && registration.visit_time < DateTime.Now.AddHours(-1))
        {
            throw new ArgumentException("预约时间不能早于当前时间");
        }
        
        if (string.IsNullOrWhiteSpace(registration.visitor_name))
        {
            throw new ArgumentException("访客姓名不能为空");
        }
        
        if (registration.visitor_name.Length > 100)
        {
            throw new ArgumentException("访客姓名不能超过100个字符");
        }

        registration.visit_type = "线上";
        registration.approval_status = "待批准";
        registration.visit_time = registration.visit_time == default ? DateTime.Now.AddDays(1) : registration.visit_time;
        _context.VisitorRegistrations.Add(registration);
        _context.SaveChanges();
        // 这里可扩展：通知工作人员有新预约
        return registration;
    }

    /// <summary>
    /// 工作人员审核预约（批准/拒绝）。
    /// </summary>
    /// <param name="visitorId">预约ID</param>
    /// <param name="approve">true=批准，false=拒绝</param>
    /// <returns>操作结果，true为成功</returns>
    public bool ApproveVisit(int visitorId, bool approve)
    {
        var reg = _context.VisitorRegistrations.Find(visitorId);
        if (reg == null) return false;
        reg.approval_status = approve ? "已批准" : "已拒绝";
        _context.SaveChanges();
        // 这里可扩展：通知家属预约状态变更
        return true;
    }

    /// <summary>
    /// 查询预约记录（可按家属、老人、状态筛选）。
    /// </summary>
    /// <param name="familyId">家属ID，可选</param>
    /// <param name="elderlyId">老人ID，可选</param>
    /// <param name="status">审批状态，可选</param>
    /// <returns>预约记录列表</returns>
    public List<VisitorRegistration> GetVisits(int? familyId = null, int? elderlyId = null, string? status = null)
    {
        var query = _context.VisitorRegistrations.AsQueryable();
        if (familyId.HasValue)
            query = query.Where(r => r.family_id == familyId.Value);
        if (elderlyId.HasValue)
            query = query.Where(r => r.elderly_id == elderlyId.Value);
        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.approval_status == status);
        return query.OrderByDescending(r => r.visit_time).ToList();
    }
} 