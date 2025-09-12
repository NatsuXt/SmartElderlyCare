using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("NURSINGPLAN")]
    public class NursingPlan
    {
        [Key]
        [Column("PLAN_ID")]
        public int PlanId { get; set; }

        [ForeignKey(nameof(Elderly))]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Column("STAFF_ID")]
        public int StaffId { get; set; }

        [Column("PLAN_START_DATE")]
        public DateTime PlanStartDate { get; set; }

        [Column("PLAN_END_DATE")]
        public DateTime PlanEndDate { get; set; }

        [Column("CARE_TYPE"), MaxLength(50)]
        public string CareType { get; set; }

        [Column("PRIORITY"), MaxLength(50)]
        public string Priority { get; set; }

        [Column("EVALUATION_STATUS"), MaxLength(50)]
        public string EvaluationStatus { get; set; }

        public ElderlyInfo Elderly { get; set; }
    }
}
