using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCare.Models
{
    [Table("ActivitySchedule")]
    public class ActivitySchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActivityId { get; set; }

        [Required]
        [StringLength(100)]
        public string ActivityName { get; set; }

        [DataType(DataType.Date)]
        public DateTime ActivityDate { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan ActivityTime { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        public int? StaffId { get; set; }

        public string ElderlyParticipants { get; set; }

        public string ActivityDescription { get; set; }

        // 导航属性
        [ForeignKey("StaffId")]
        public StaffInfo Staff { get; set; }

        //public ICollection<ActivityParticipation> ActivityParticipations { get; set; }
    }
}