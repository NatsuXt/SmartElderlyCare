using System;
using System.Collections.Generic;

namespace ElderlyCareSystem.Dtos.Mobile
{
    // 请求 DTO：通过 FamilyId 查询关联老人
    public class FamilyQueryRequestDto
    {
        public int FamilyId { get; set; }
    }

    // 响应 DTO：老人基本信息
    public class ElderlyBasicInfoDto
    {
        public int ElderlyId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string IdCardNumber { get; set; }
        public string ContactPhone { get; set; }
    }

    // 响应 DTO：健康监测信息
    public class HealthMonitoringDto
    {
        public DateTime MonitorTime { get; set; }
        public string BloodPressure { get; set; }
        public int? HeartRate { get; set; }
        public string Temperature { get; set; }
        public string Notes { get; set; }
    }

    // 响应 DTO：账单信息
    public class FeeSettlementDto
    {
        public DateTime BillingDate { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }

    // 响应 DTO：活动参与记录
    public class ActivityParticipationDto
    {
        public string ActivityName { get; set; }
        public DateTime ParticipationDate { get; set; }
        public string Feedback { get; set; }
    }

    // 响应 DTO：最终聚合结果
    public class FamilyElderlyDetailDto
    {
        public ElderlyBasicInfoDto BasicInfo { get; set; }
        public List<HealthMonitoringDto> HealthMonitorings { get; set; }
        public List<FeeSettlementDto> FeeSettlements { get; set; }
        public List<ActivityParticipationDto> ActivityParticipations { get; set; }
    }

    // 操作日志 DTO
    public class OperationLogDto
    {
        public int FamilyId { get; set; }
        public int ElderlyId { get; set; }
        public string OperationType { get; set; }
        public DateTime OperationTime { get; set; }
        public string Description { get; set; }
    }
}
