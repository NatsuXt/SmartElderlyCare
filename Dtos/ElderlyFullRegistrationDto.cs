using System;
using System.Collections.Generic;

namespace ElderlyCareSystem.Dtos
{
    public class ElderlyFullRegistrationDtos
    {
        public ElderlyInfoDto Elderly { get; set; }
        public HealthAssessmentReportDto Assessment { get; set; }
        public HealthMonitoringDto Monitoring { get; set; }
        public List<FamilyInfoDto> Families { get; set; }
       
    }
}
