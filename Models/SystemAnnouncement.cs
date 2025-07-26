using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCare.Models
{
    [Table("SystemAnnouncements")]
    public class SystemAnnouncement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnnouncementId { get; set; }

        [DataType(DataType.Date)]
        public DateTime AnnouncementDate { get; set; }

        [StringLength(50)]
        public string AnnouncementType { get; set; }

        public string AnnouncementContent { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        [StringLength(50)]
        public string Audience { get; set; }

        public int? CreatedById { get; set; }

        public string ReadStatus { get; set; }

        public string Comments { get; set; }

        // 导航属性
        [ForeignKey("CreatedById")]
        public StaffInfo CreatedBy { get; set; }
    }
}