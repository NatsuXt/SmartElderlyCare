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

        [ForeignKey(nameof(Elderly))]
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

        [Column("HEALTH_GRADE"), MaxLength(50)]
        public string? HealthGrade { get; set; }

        public ElderlyInfo Elderly { get; set; }
    }
}
