using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("EMERGENCYSOS")]
    public class EmergencySOS
    {
        [Key]
        [Column("CALL_ID")]
        public int CallId { get; set; }

        [ForeignKey(nameof(Elderly))]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Column("CALL_TIME")]
        public DateTime CallTime { get; set; }

        [Column("CALL_TYPE"), MaxLength(50)]
        public string CallType { get; set; }

        [Column("ROOM_ID")]
        public int RoomId { get; set; }

        [Column("RESPONSE_TIME")]
        public DateTime? ResponseTime { get; set; }

        [Column("RESPONSE_STAFF")]
        public int? ResponseStaff { get; set; }

        [Column("FOLLOW_UP_REQUIRED"), MaxLength(1)]
        public string FollowUpRequired { get; set; }  // 'Y' or 'N'

        [Column("CALL_STATUS"), MaxLength(20)]
        public string CallStatus { get; set; }

        [Column("HANDLING_RESULT")]
        public string HandlingResult { get; set; }

        public ElderlyInfo Elderly { get; set; }
    }
}
