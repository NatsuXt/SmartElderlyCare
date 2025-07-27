// NursingPlan.cs
namespace ElderlyCareManagement.Models
{
    public class NursingPlan
    {
        public int PlanId { get; set; }
        public int ElderlyId { get; set; }
        public int? StaffId { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public string CareType { get; set; }
        public string Priority { get; set; }
        public string EvaluationStatus { get; set; }
        
        // Navigation properties
        //public ElderlyInfo Elderly { get; set; }
        public StaffInfo Staff { get; set; }
    }
}