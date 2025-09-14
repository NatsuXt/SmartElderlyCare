using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using ElderlyCareSystem.Models;

public class VisitorRegistrationService
{
    private readonly AppDbContext _context;

    public VisitorRegistrationService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 访客提交远程视频探视预约申请。
    /// </summary>
    /// <param name="request">预约申请请求</param>
    /// <returns>新建的预约记录</returns>
    public VisitorRegistration SubmitVideoVisit(VideoVisitRequest request)
    {
        try
        {
            // 记录调试信息
            Console.WriteLine($"开始处理预约申请：负责人ID={request.ResponsibleVisitorId}, 访客姓名={request.VisitorName}");

            // 验证负责人访客ID是否存在
            var responsibleVisitor = _context.VisitorLogins.FirstOrDefault(v => v.VisitorId == request.ResponsibleVisitorId);
            if (responsibleVisitor == null)
            {
                Console.WriteLine($"负责人访客ID {request.ResponsibleVisitorId} 不存在");
                throw new ArgumentException("负责人访客ID不存在");
            }

            Console.WriteLine($"找到负责人：{responsibleVisitor.VisitorName}, 为访客 {request.VisitorName} 提交预约");

            // 处理老人ID：支持指定老人和全体老人两种模式
            int elderlyId = 0; // 默认值，0表示全体老人
            string visitTypeDescription = "";
            
            if (request.ElderlyId > 0)
            {
                // 模式1：指定老人
                elderlyId = request.ElderlyId;
                visitTypeDescription = $"指定老人ID {elderlyId}";
                
                // 验证老人ID是否存在
                var elderly = _context.ElderlyInfos.FirstOrDefault(e => e.ElderlyId == elderlyId);
                if (elderly == null)
                {
                    Console.WriteLine($"老人ID {elderlyId} 不存在");
                    throw new ArgumentException($"指定的老人ID {elderlyId} 不存在");
                }
                
                Console.WriteLine($"找到老人：{elderly.Name}");
            }
            else
            {
                // 模式2：全体老人
                elderlyId = 0;
                visitTypeDescription = "全体老人";
            }

            // 创建预约记录
            var registration = new VisitorRegistration
            {
                VisitorId = request.ResponsibleVisitorId, // 存储负责人ID
                ElderlyId = elderlyId,
                VisitorName = request.VisitorName, // 存储实际访客姓名
                // VisitorPhone = request.VisitorPhone, // 临时注释掉，等数据库添加字段后再启用
                VisitTime = DateTime.Now, // 预约申请时间
                RelationshipToElderly = request.RelationshipToElderly,
                VisitReason = $"{request.VisitReason} (探视对象: {visitTypeDescription})",
                VisitType = request.VisitType, // 使用请求中指定的访问类型
                ApprovalStatus = "待批准"
            };

            Console.WriteLine($"准备保存预约记录：访客={registration.VisitorName}, 老人ID={registration.ElderlyId}");

            _context.VisitorRegistrations.Add(registration);
            _context.SaveChanges();
            
            Console.WriteLine($"预约记录保存成功，ID={registration.RegistrationId}");
            
            // 这里可扩展：通知工作人员有新预约
            return registration;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"提交预约申请失败：{ex.Message}");
            Console.WriteLine($"完整异常信息：{ex}");
            throw; // 重新抛出异常
        }
    }

    /// <summary>
    /// 负责人批量提交视频探视预约申请。
    /// </summary>
    /// <param name="request">批量预约申请请求</param>
    /// <returns>新建的预约记录列表</returns>
    public List<VisitorRegistration> SubmitBulkVideoVisit(BulkVideoVisitRequest request)
    {
        try
        {
            Console.WriteLine($"开始处理批量预约申请：负责人ID={request.ResponsibleVisitorId}, 访客数量={request.Visitors.Count}");

            // 验证负责人访客ID是否存在
            var responsibleVisitor = _context.VisitorLogins.FirstOrDefault(v => v.VisitorId == request.ResponsibleVisitorId);
            if (responsibleVisitor == null)
            {
                Console.WriteLine($"负责人访客ID {request.ResponsibleVisitorId} 不存在");
                throw new ArgumentException("负责人访客ID不存在");
            }

            Console.WriteLine($"找到负责人：{responsibleVisitor.VisitorName}，准备为 {request.Visitors.Count} 个访客创建预约");

            // 处理老人ID：支持指定老人和全体老人两种模式
            int elderlyId = 0;
            string visitTypeDescription = "";
            
            if (request.ElderlyId > 0)
            {
                elderlyId = request.ElderlyId;
                visitTypeDescription = $"指定老人ID {elderlyId}";
                
                // 验证老人ID是否存在
                var elderly = _context.ElderlyInfos.FirstOrDefault(e => e.ElderlyId == elderlyId);
                if (elderly == null)
                {
                    Console.WriteLine($"老人ID {elderlyId} 不存在");
                    throw new ArgumentException($"指定的老人ID {elderlyId} 不存在");
                }
                
                Console.WriteLine($"找到老人：{elderly.Name}");
            }
            else
            {
                elderlyId = 0;
                visitTypeDescription = "全体老人";
                
                // 验证全体老人记录是否存在
                var allElderly = _context.ElderlyInfos.FirstOrDefault(e => e.ElderlyId == 0);
                if (allElderly == null)
                {
                    Console.WriteLine("全体老人记录(ID=0)不存在");
                    throw new ArgumentException("全体老人记录不存在，请联系管理员添加 ELDERLY_ID=0 的记录");
                }
                
                Console.WriteLine("找到全体老人记录");
            }

            var registrations = new List<VisitorRegistration>();
            var currentTime = DateTime.Now;

            // 为每个访客创建预约记录
            foreach (var visitor in request.Visitors)
            {
                var registration = new VisitorRegistration
                {
                    VisitorId = request.ResponsibleVisitorId, // 存储负责人ID
                    ElderlyId = elderlyId,
                    VisitorName = visitor.VisitorName, // 存储实际访客姓名
                    // VisitorPhone = visitor.VisitorPhone, // 临时注释掉，等数据库添加字段后再启用
                    VisitTime = currentTime, // 预约申请时间
                    RelationshipToElderly = visitor.RelationshipToElderly,
                    VisitReason = $"{visitor.VisitReason} (探视对象: {visitTypeDescription})",
                    VisitType = request.VisitType, // 使用请求中指定的访问类型
                    ApprovalStatus = "待批准"
                };

                registrations.Add(registration);
                Console.WriteLine($"创建预约记录：访客={registration.VisitorName}");
            }

            // 批量保存到数据库
            _context.VisitorRegistrations.AddRange(registrations);
            _context.SaveChanges();
            
            Console.WriteLine($"批量预约记录保存成功，共创建 {registrations.Count} 条记录");
            
            return registrations;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"提交批量预约申请失败：{ex.Message}");
            Console.WriteLine($"完整异常信息：{ex}");
            throw;
        }
    }

    /// <summary>
    /// 工作人员审核预约（批准/拒绝）。
    /// </summary>
    /// <param name="registrationId">预约ID</param>
    /// <param name="approve">true=批准，false=拒绝</param>
    /// <returns>操作结果，true为成功</returns>
    public bool ApproveVisit(int registrationId, bool approve)
    {
        var reg = _context.VisitorRegistrations.Find(registrationId);
        if (reg == null) return false;
        reg.ApprovalStatus = approve ? "已批准" : "已拒绝";
        _context.SaveChanges();
        // 这里可扩展：通知访客预约状态变更
        return true;
    }

    /// <summary>
    /// 工作人员一键批准某个负责人的所有待审核预约申请。
    /// </summary>
    /// <param name="responsibleVisitorId">负责人访客ID</param>
    /// <returns>批准的预约记录数量</returns>
    public int ApproveAllVisitsByResponsiblePerson(int responsibleVisitorId)
    {
        try
        {
            Console.WriteLine($"开始批量批准负责人ID={responsibleVisitorId}的所有待审核预约");

            // 验证负责人访客ID是否存在
            var responsibleVisitor = _context.VisitorLogins.FirstOrDefault(v => v.VisitorId == responsibleVisitorId);
            if (responsibleVisitor == null)
            {
                Console.WriteLine($"负责人访客ID {responsibleVisitorId} 不存在");
                throw new ArgumentException("负责人访客ID不存在");
            }

            // 查找该负责人所有待审核的预约申请
            var pendingRegistrations = _context.VisitorRegistrations
                .Where(r => r.VisitorId == responsibleVisitorId && r.ApprovalStatus == "待批准")
                .ToList();

            if (pendingRegistrations.Count == 0)
            {
                Console.WriteLine($"负责人 {responsibleVisitor.VisitorName} 没有待审核的预约申请");
                return 0;
            }

            Console.WriteLine($"找到负责人 {responsibleVisitor.VisitorName} 的 {pendingRegistrations.Count} 条待审核预约");

            // 批量更新状态为已批准
            foreach (var registration in pendingRegistrations)
            {
                registration.ApprovalStatus = "已批准";
                Console.WriteLine($"批准预约：ID={registration.RegistrationId}, 访客={registration.VisitorName}");
            }

            // 保存更改
            var approvedCount = _context.SaveChanges();
            Console.WriteLine($"成功批准 {pendingRegistrations.Count} 条预约申请");

            // 这里可扩展：批量通知访客预约状态变更
            return pendingRegistrations.Count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"批量批准预约申请失败：{ex.Message}");
            Console.WriteLine($"完整异常信息：{ex}");
            throw;
        }
    }

    /// <summary>
    /// 查询预约记录（可按访客、老人、状态筛选）。
    /// </summary>
    /// <param name="visitorId">访客ID，可选</param>
    /// <param name="elderlyId">老人ID，可选</param>
    /// <param name="status">审批状态，可选</param>
    /// <returns>预约记录列表</returns>
    public List<VisitorRegistration> GetVisits(int? visitorId = null, int? elderlyId = null, string? status = null)
    {
        var query = _context.VisitorRegistrations.AsQueryable();
        if (visitorId.HasValue)
            query = query.Where(r => r.VisitorId == visitorId.Value);
        if (elderlyId.HasValue)
            query = query.Where(r => r.ElderlyId == elderlyId.Value);
        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.ApprovalStatus == status);
        return query.OrderByDescending(r => r.VisitTime).ToList();
    }
} 