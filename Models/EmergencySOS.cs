using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCare.Models
{
    [Table("EmergencySOS")]
    public class EmergencySOS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CallId { get; set; }

        [Required]
        public int ElderlyId { get; set; }

        [Required]
        public DateTime CallTime { get; set; }

        [StringLength(50)]
        public string CallType { get; set; }

        public int? RoomId { get; set; }

        public DateTime? ResponseTime { get; set; }

        public int? ResponseStaffId { get; set; }

        public bool FollowUpRequired { get; set; }

        [StringLength(20)]
        public string CallStatus { get; set; }

        public string HandlingResult { get; set; }

        // 导航属性
        //[ForeignKey("ElderlyId")]
        //public ElderlyInfo Elderly { get; set; }

        [ForeignKey("ResponseStaffId")]
        public StaffInfo ResponseStaff { get; set; }

        //[ForeignKey("RoomId")]
        //public RoomManagement Room { get; set; }
    }
}