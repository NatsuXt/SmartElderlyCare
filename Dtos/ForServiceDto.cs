using System;
using System.Collections.Generic;

namespace ElderlyCareSystem.Dtos
{
    //对应功能●	1.1.20 老人电子档案管理
    public class ElderlyFullRecordDtos
    {
        public ElderlyInfoDto ElderlyInfo { get; set; }
        public List<FamilyInfoDto> FamilyInfos { get; set; }
        public List<HealthMonitoringDto> HealthMonitorings { get; set; }
        public List<HealthAssessmentReportDto> HealthAssessments { get; set; }
        public List<MedicalOrderDto> MedicalOrders { get; set; }
        public List<NursingPlanDto> NursingPlans { get; set; }
        public List<FeeSettlementDto> FeeSettlements { get; set; }
        public List<ActivityParticipationDto> ActivityParticipations { get; set; }
    }
    //对应功能●	1.1.1 老人入住登记与健康评估
    public class ElderlyFullRegistrationDtos
    {
        public ElderlyInfoDto Elderly { get; set; }
        public HealthAssessmentReportDto Assessment { get; set; }
        public HealthMonitoringDto Monitoring { get; set; }
        public List<FamilyInfoDto> Families { get; set; }

    }
    //对应功能●	1.1.15 移动端家属信息查询中的第一项
    public class FamilyLoginRequestDto
    {
        public string FamilyName { get; set; }
        public string ContactPhone { get; set; }
        public string ElderlyName { get; set; }
        public string ElderlyIdCardNumber { get; set; }
    }
    //对应功能●	1.1.15 移动端家属信息查询中的第二项

    public class FamilyQueryResponseDto
    {
        public ElderlyInfoDto ElderlyInfo { get; set; }
        public List<HealthMonitoringDto> HealthMonitorings { get; set; }
        public List<FeeSettlementDto> FeeSettlements { get; set; }
        public List<ActivityParticipationDto> ActivityParticipations { get; set; }
    }
    public class DietRecommendationResponseDto
    {
        public int RecommendationId { get; set; }
        public DateTime RecommendationDate { get; set; }
        public string RecommendedFood { get; set; }
        public string ExecutionStatus { get; set; }
    }
    public class DietExecutionUpdateDto
    {
        public int RecommendationId { get; set; }
        public string ExecutionStatus { get; set; }
    }
}
