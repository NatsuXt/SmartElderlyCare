using System;
using System.Collections.Generic;
using System.Linq;

public class SystemAnnouncementService
{
    private readonly AppDbContext _context;
    private readonly OperationLogService _logService;

    public SystemAnnouncementService(AppDbContext context, OperationLogService logService)
    {
        _context = context;
        _logService = logService;
    }

    /// <summary>
    /// 查询公告列表（可按受众、状态筛选）。
    /// </summary>
    /// <param name="audience">受众类型，如"家属"、"员工"，为空则查询全部</param>
    /// <param name="status">公告状态，如"已发布"，为空则查询全部</param>
    /// <returns>公告列表</returns>
    public List<SystemAnnouncements> GetAnnouncements(string? audience = null, string status = "已发布")
    {
        var query = _context.SystemAnnouncements.AsQueryable();
        if (!string.IsNullOrEmpty(audience))
            query = query.Where(a => a.audience == audience || a.audience == "全体");
        if (!string.IsNullOrEmpty(status))
            query = query.Where(a => a.status == status);
        return query.OrderByDescending(a => a.announcement_date).ToList();
    }

    /// <summary>
    /// 发布新公告。
    /// </summary>
    /// <param name="ann">公告实体，包含公告内容、类型、受众等</param>
    /// <param name="staffId">发布人ID</param>
    /// <returns>新发布的公告信息</returns>
    public SystemAnnouncements Publish(SystemAnnouncements ann, int staffId)
    {
        ann.announcement_date = DateTime.Now;
        ann.status = "已发布";
        ann.created_by = staffId;
        ann.read_status = "未阅读"; // 设置默认阅读状态
        ann.comments = ""; // 设置默认评论为空字符串
        _context.SystemAnnouncements.Add(ann);
        _context.SaveChanges();
        _logService.Create(new OperationLog
        {
            operation_description = $"发布公告：{ann.announcement_type}-{ann.announcement_content}",
            staff_id = staffId,
            operation_time = ann.announcement_date,
            operation_type = "发布公告",
            operation_status = "成功",
            affected_entity = "SystemAnnouncements",
            ip_address = "系统内部",
            device_type = "服务器"
        }, out bool _);
        return ann;
    }

    /// <summary>
    /// 撤回/失效公告（将状态设为草稿或已撤回）。
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <param name="staffId">操作人ID</param>
    /// <returns>操作结果，true为成功，false为失败</returns>
    public bool Deactivate(int id, int staffId)
    {
        var ann = _context.SystemAnnouncements.Find(id);
        if (ann == null) return false;
        ann.status = "草稿";
        _context.SaveChanges();
        _logService.Create(new OperationLog
        {
            operation_description = $"撤回公告：{ann.announcement_type}-{ann.announcement_content}",
            staff_id = staffId,
            operation_time = DateTime.Now,
            operation_type = "撤回公告",
            operation_status = "成功",
            affected_entity = "SystemAnnouncements",
            ip_address = "系统内部",
            device_type = "服务器"
        }, out bool _);
        return true;
    }

    /// <summary>
    /// 给公告添加评论。
    /// </summary>
    /// <param name="announcementId">公告ID</param>
    /// <param name="comment">评论内容</param>
    /// <returns>操作结果，true为成功，false为失败</returns>
    public bool AddComment(int announcementId, string comment)
    {
        var ann = _context.SystemAnnouncements.Find(announcementId);
        if (ann == null) return false;
        ann.comments = comment;
        _context.SaveChanges();
        return true;
    }

    /// <summary>
    /// 标记公告为已读。
    /// </summary>
    /// <param name="announcementId">公告ID</param>
    /// <param name="readStatus">阅读状态，默认"已阅读"</param>
    /// <returns>操作结果，true为成功，false为失败</returns>
    public bool MarkAsRead(int announcementId, string readStatus = "已阅读")
    {
        var ann = _context.SystemAnnouncements.Find(announcementId);
        if (ann == null) return false;
        ann.read_status = readStatus;
        _context.SaveChanges();
        return true;
    }
} 