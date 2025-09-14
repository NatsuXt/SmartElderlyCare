using System;
using System.Collections.Generic;
using System.Linq;

public class OperationLogService
{
    private readonly AppDbContext _context;

    public OperationLogService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有操作日志。
    /// </summary>
    /// <returns>操作日志列表</returns>
    public List<OperationLog> GetAll()
    {
        return _context.OperationLogs.OrderByDescending(log => log.operation_time).ToList();
    }

    /// <summary>
    /// 根据ID获取单条操作日志。
    /// </summary>
    /// <param name="id">日志ID</param>
    /// <returns>操作日志详情</returns>
    public OperationLog? GetById(int id)
    {
        return _context.OperationLogs.Find(id);
    }

    /// <summary>
    /// 按条件筛选操作日志。
    /// </summary>
    /// <param name="days">查询最近几天的记录</param>
    /// <param name="operationType">操作类型筛选</param>
    /// <param name="staffId">员工ID筛选</param>
    /// <param name="operationStatus">操作状态筛选</param>
    /// <returns>筛选后的操作日志列表</returns>
    public List<OperationLog> GetLogs(int? days = null, string[]? operationType = null, int? staffId = null, string? operationStatus = null)
    {
        var query = _context.OperationLogs.AsQueryable();

        // 按时间筛选
        if (days.HasValue && days.Value > 0)
        {
            var startDate = DateTime.Now.AddDays(-days.Value);
            query = query.Where(log => log.operation_time >= startDate);
        }

        // 按操作类型筛选
        if (operationType != null && operationType.Length > 0)
        {
            query = query.Where(log => operationType.Contains(log.operation_type));
        }

        // 按员工ID筛选
        if (staffId.HasValue)
        {
            query = query.Where(log => log.staff_id == staffId.Value);
        }

        // 按操作状态筛选
        if (!string.IsNullOrEmpty(operationStatus))
        {
            query = query.Where(log => log.operation_status == operationStatus);
        }

        return query.OrderByDescending(log => log.operation_time).ToList();
    }

    /// <summary>
    /// 创建一条新的操作日志，自动补全时间，校验内容并检测敏感操作。
    /// </summary>
    /// <param name="log">操作日志实体</param>
    /// <param name="isSensitive">输出参数，是否为敏感操作</param>
    /// <returns>新建的操作日志</returns>
    /// <exception cref="ArgumentException">操作描述为空或超长时抛出</exception>
    public OperationLog Create(OperationLog log, out bool isSensitive)
    {
        // 校验操作描述
        if (string.IsNullOrWhiteSpace(log.operation_description))
            throw new ArgumentException("操作描述不能为空");
        if (log.operation_description.Length > 500) // 增加长度限制
            throw new ArgumentException("操作描述不能超过500个字符");

        // 敏感操作告警
        isSensitive = false;
        if (log.operation_description.Contains("删除") || 
            log.operation_description.Contains("敏感") ||
            log.operation_description.Contains("拒绝") ||
            log.operation_type == "审核预约")
        {
            isSensitive = true;
        }

        log.operation_time = DateTime.Now;
        _context.OperationLogs.Add(log);
        _context.SaveChanges();
        return log;
    }

    /// <summary>
    /// 创建操作日志的安全版本，返回错误信息而不抛出异常
    /// </summary>
    /// <param name="log">操作日志实体</param>
    /// <param name="errorMessage">错误信息</param>
    /// <returns>成功返回日志对象，失败返回null</returns>
    public OperationLog? CreateSafe(OperationLog log, out string? errorMessage)
    {
        errorMessage = null;
        try
        {
            var result = Create(log, out _);
            return result;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return null;
        }
    }

    /// <summary>
    /// 记录异常日志。
    /// </summary>
    /// <param name="staffId">操作人ID</param>
    /// <param name="errorMsg">异常信息</param>
    public void LogException(int staffId, string errorMsg)
    {
        var log = new OperationLog
        {
            staff_id = staffId,
            operation_time = DateTime.Now,
            operation_type = "异常",
            operation_description = $"异常：{errorMsg}",
            operation_status = "失败"
        };
        _context.OperationLogs.Add(log);
        _context.SaveChanges();
    }

    /// <summary>
    /// 获取操作统计信息
    /// </summary>
    /// <param name="days">统计最近几天的数据</param>
    /// <returns>统计信息</returns>
    public object GetStatistics(int days = 7)
    {
        var startDate = DateTime.Now.AddDays(-days);
        var logs = _context.OperationLogs.Where(log => log.operation_time >= startDate);

        return new
        {
            TotalCount = logs.Count(),
            SuccessCount = logs.Count(log => log.operation_status == "成功"),
            FailureCount = logs.Count(log => log.operation_status == "失败"),
            OperationTypes = logs.GroupBy(log => log.operation_type)
                                .Select(g => new { Type = g.Key, Count = g.Count() })
                                .OrderByDescending(x => x.Count)
                                .ToList(),
            DailyCount = logs.GroupBy(log => log.operation_time.Date)
                            .Select(g => new { Date = g.Key, Count = g.Count() })
                            .OrderBy(x => x.Date)
                            .ToList()
        };
    }

    /// <summary>
    /// 禁止修改操作日志。
    /// </summary>
    /// <returns>始终为 false</returns>
    public bool Update(int id, OperationLog log) => false;

    /// <summary>
    /// 禁止删除操作日志。
    /// </summary>
    /// <returns>始终为 false</returns>
    public bool Delete(int id) => false;
}
