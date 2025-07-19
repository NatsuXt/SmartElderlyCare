using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("HEALTHASSESSMENTREPORT")]
    public class HealthAssessmentReport
    {
        [Key]
        [Column("ASSESSMENT_ID")]
        public int AssessmentId { get; set; }

        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Column("ASSESSMENT_DATE")]
        public DateTime AssessmentDate { get; set; }

        [Column("PHYSICAL_HEALTH_FUNCTION")]
        public int PhysicalHealthFunction { get; set; }

        [Column("PSYCHOLOGICAL_FUNCTION")]
        public int PsychologicalFunction { get; set; }

        [Column("COGNITIVE_FUNCTION")]
        public int CognitiveFunction { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("HEALTH_GRADE")]
        public string HealthGrade { get; set; }

       
    }
}
