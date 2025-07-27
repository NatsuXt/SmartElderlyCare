// DTOs/NursingPlanDTO.cs
namespace ElderlyCareManagement.DTOs
{
    public class NursingPlanDTO
    {
        public int PlanId { get; set; }
        public int ElderlyId { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public string CareType { get; set; }
        public string Priority { get; set; }
    }

    public class NursingPlanCreateDTO
    {
        public int ElderlyId { get; set; }
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public string CareType { get; set; }
        public string Priority { get; set; }
    }

    public class NursingPlanDetailDTO : NursingPlanDTO
    {
        public int? StaffId { get; set; }
        public string StaffName { get; set; }
        public string EvaluationStatus { get; set; }
    }
}