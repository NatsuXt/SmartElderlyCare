using System.Collections.Generic;

// ElderlyDto.cs
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
}

// HealthAssessmentDto.cs
namespace ElderlyCareSystem.Dtos
{
    public class HealthAssessmentDto
    {
        public int PhysicalHealthFunction { get; set; }
        public int PsychologicalFunction { get; set; }
        public int CognitiveFunction { get; set; }
        public string HealthGrade { get; set; }
    }
}

// HealthMonitoringDto.cs
namespace ElderlyCareSystem.Dtos
{
    public class HealthMonitoringDto
    {
        public int HeartRate { get; set; }
        public string BloodPressure { get; set; }
        public float OxygenLevel { get; set; }
        public float Temperature { get; set; }
        public string Status { get; set; }
    }
}

// FamilyDto.cs
namespace ElderlyCareSystem.Dtos
{
    public class FamilyDto
    {
        public string Name { get; set; }
        public string Relationship { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Address { get; set; }
        public string IsPrimaryContact { get; set; }
    }
}

// ElderlyFullRegistrationDto.cs

namespace ElderlyCareSystem.Dtos
{
    public class ElderlyFullRegistrationDto
    {
        public ElderlyDto Elderly { get; set; }
        public HealthAssessmentDto Assessment { get; set; }
        public HealthMonitoringDto Monitoring { get; set; }
        public List<FamilyDto> Families { get; set; }
    }
}
