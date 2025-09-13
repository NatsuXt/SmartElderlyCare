using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("ACTIVITYPARTICIPATION")]
    public class ActivityParticipation
    {
        [Key]
        [Column("PARTICIPATION_ID")]
        public int ParticipationId { get; set; }

        [Column("ACTIVITY_ID")]
        public int ActivityId { get; set; }

        [ForeignKey(nameof(Elderly))]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Column("STATUS"), MaxLength(20)]
        public string Status { get; set; }

        [Column("REGISTRATION_TIME")]
        public DateTime RegistrationTime { get; set; }

        [Column("CHECK_IN_TIME")]
        public DateTime? CheckInTime { get; set; }

        [Column("FEEDBACK")]
        public string? Feedback { get; set; }

        public ElderlyInfo Elderly { get; set; }
    }
}
