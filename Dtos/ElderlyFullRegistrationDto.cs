using System;
using System.Collections.Generic;

namespace ElderlyCareSystem.Dtos
{
    public class ElderlyDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string IdCardNumber { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }
    }

    public class FamilyDto
    {
        public string Name { get; set; }
        public string Relationship { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string IsPrimaryContact { get; set; }
    }

    public class HealthAssessmentDto
    {
        public int PhysicalHealthFunction { get; set; }
        public int PsychologicalFunction { get; set; }
        public int CognitiveFunction { get; set; }
        public string HealthGrade { get; set; }
        public DateTime AssessmentDate { get; set; }
    }

    public class HealthMonitoringDto
    {
        public int HeartRate { get; set; }
        public string BloodPressure { get; set; }
        public float OxygenLevel { get; set; }
        public float Temperature { get; set; }
        public string Status { get; set; }
        public DateTime MonitoringDate { get; set; }
    }


    public class ElderlyFullRegistrationDto
    {
        public ElderlyDto Elderly { get; set; }
        public HealthAssessmentDto Assessment { get; set; }
        public HealthMonitoringDto Monitoring { get; set; }
        public List<FamilyDto> Families { get; set; }
       
    }
}
