using System;
using System.Collections.Generic;

namespace ElderlyCareSystem.Dtos
{
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

}
