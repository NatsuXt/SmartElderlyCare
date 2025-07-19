using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("HealthAssessmentReport")]
    public class HealthAssessmentReport
    {
        [Key]
        [Column("assessment_id")]
        public int AssessmentId { get; set; }

        [Column("elderly_id")]
        public int ElderlyId { get; set; }

        [Column("assessment_date")]
        public DateTime AssessmentDate { get; set; }

        [Column("Physical_health_function")]
        public int PhysicalHealthFunction { get; set; }

        [Column("Psychological_function")]
        public int PsychologicalFunction { get; set; }

        [Column("Cognitive_function")]
        public int CognitiveFunction { get; set; }

        [Column("Health_grade")]
        public string HealthGrade { get; set; }
    }
}