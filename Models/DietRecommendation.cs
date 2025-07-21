using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("DIETRECOMMENDATION")]
    public class DietRecommendation
    {
        [Key]
        [Column("RECOMMENDATION_ID")]
        public int RecommendationId { get; set; }

        [ForeignKey(nameof(Elderly))]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Column("RECOMMENDATION_DATE")]
        public DateTime RecommendationDate { get; set; }

        [Column("RECOMMENDED_FOOD")]
        public string RecommendedFood { get; set; }

        [Column("EXECUTION_STATUS"), MaxLength(20)]
        public string ExecutionStatus { get; set; }

        public ElderlyInfo Elderly { get; set; }
    }
}
