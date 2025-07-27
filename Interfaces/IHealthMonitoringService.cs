using System.Collections.Generic;
using RoomDeviceManagement.Models;

namespace RoomDeviceManagement.Interfaces
{
    /// <summary>
    /// 健康监测服务接口
    /// </summary>
    public interface IHealthMonitoringService
    {
        /// <summary>
        /// 获取所有健康监测记录
        /// </summary>
        /// <returns>健康监测记录列表</returns>
        List<HealthMonitoring> GetAllHealthRecords();

        /// <summary>
        /// 根据老人ID获取健康监测记录
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <returns>健康监测记录列表</returns>
        List<HealthMonitoring> GetHealthRecordsByElderlyId(string elderlyId);

        /// <summary>
        /// 根据监测类型获取记录
        /// </summary>
        /// <param name="monitoringType">监测类型</param>
        /// <returns>健康监测记录列表</returns>
        List<HealthMonitoring> GetHealthRecordsByType(string monitoringType);

        /// <summary>
        /// 获取异常健康记录
        /// </summary>
        /// <returns>异常健康记录列表</returns>
        List<HealthMonitoring> GetAbnormalHealthRecords();

        /// <summary>
        /// 获取未处理的异常记录
        /// </summary>
        /// <returns>未处理的异常记录列表</returns>
        List<HealthMonitoring> GetUnprocessedAbnormalRecords();

        /// <summary>
        /// 添加健康监测记录
        /// </summary>
        /// <param name="record">健康监测记录</param>
        /// <returns>操作是否成功</returns>
        bool AddHealthRecord(HealthMonitoring record);

        /// <summary>
        /// 批量添加健康监测记录
        /// </summary>
        /// <param name="records">健康监测记录列表</param>
        /// <returns>操作是否成功</returns>
        bool AddHealthRecords(List<HealthMonitoring> records);

        /// <summary>
        /// 更新健康监测记录
        /// </summary>
        /// <param name="record">健康监测记录</param>
        /// <returns>操作是否成功</returns>
        bool UpdateHealthRecord(HealthMonitoring record);

        /// <summary>
        /// 处理异常健康记录
        /// </summary>
        /// <param name="recordId">记录ID</param>
        /// <param name="processor">处理人员</param>
        /// <param name="measures">处理措施</param>
        /// <returns>操作是否成功</returns>
        bool ProcessAbnormalRecord(string recordId, string processor, string measures);

        /// <summary>
        /// 获取老人最新健康状态
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <returns>最新健康监测记录字典</returns>
        Dictionary<string, HealthMonitoring> GetLatestHealthStatus(string elderlyId);

        /// <summary>
        /// 检查健康数据是否异常
        /// </summary>
        /// <param name="value">监测值</param>
        /// <param name="minNormal">正常范围最小值</param>
        /// <param name="maxNormal">正常范围最大值</param>
        /// <returns>是否异常</returns>
        bool IsHealthDataAbnormal(decimal value, decimal minNormal, decimal maxNormal);

        /// <summary>
        /// 获取健康数据统计
        /// </summary>
        /// <param name="elderlyId">老人ID</param>
        /// <param name="monitoringType">监测类型</param>
        /// <param name="days">统计天数</param>
        /// <returns>统计结果</returns>
        Dictionary<string, object> GetHealthDataStatistics(string elderlyId, string monitoringType, int days = 7);
    }
}
