using System;
using System.Collections.Generic;

namespace ElderlyCareSystem.Dtos
{
    //对应功能●	1.1.20 老人电子档案管理
    public class ElderlyFullRecordDtos
    {
        public ElderlyInfoDto ElderlyInfo { get; set; }
        public List<FamilyInfoCreateDto> FamilyInfos { get; set; }
        public List<HealthMonitoringCreateDto> HealthMonitorings { get; set; }
        public List<HealthAssessmentReportCreateDto> HealthAssessments { get; set; }
        public List<MedicalOrderDto> MedicalOrders { get; set; }
        public List<NursingPlanDto> NursingPlans { get; set; }
        public List<FeeSettlementDto> FeeSettlements { get; set; }
        public List<ActivityParticipationDto> ActivityParticipations { get; set; }
    }
    //对应功能●	1.1.1 老人入住登记与健康评估
    public class ElderlyFullRegistrationDtos
    {
        public ElderlyInfoCreateDto Elderly { get; set; }
        public HealthAssessmentReportCreateDto Assessment { get; set; }
        public HealthMonitoringCreateDto Monitoring { get; set; }
        public List<FamilyInfoCreateDto> Families { get; set; }

    }
    //对应功能●	1.1.15 移动端家属信息查询中的第一项
    public class FamilyLoginRequestDto
    {
        public string FamilyName { get; set; }
        public string ContactPhone { get; set; }
        public string ElderlyName { get; set; }
        public string ElderlyIdCardNumber { get; set; }
    }
    
    
  
}
