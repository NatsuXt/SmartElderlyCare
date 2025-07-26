using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCare.Models
{
    [Table("NursingPlan")]
    public class NursingPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlanId { get; set; }

        [Required]
        public int ElderlyId { get; set; }

        public int? StaffId { get; set; }

        [DataType(DataType.Date)]
        public DateTime PlanStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime PlanEndDate { get; set; }

        [StringLength(50)]
        public string CareType { get; set; }

        [StringLength(50)]
        public string Priority { get; set; }

        [StringLength(50)]
        public string EvaluationStatus { get; set; }

        // 导航属性
        //[ForeignKey("ElderlyId")]
        //public ElderlyInfo Elderly { get; set; }

        [ForeignKey("StaffId")]
        public StaffInfo Staff { get; set; }
    }
}