using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCare.Models
{
    [Table("OperationLog")]
    public class OperationLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        public int? StaffId { get; set; }

        [Required]
        public DateTime OperationTime { get; set; }

        [StringLength(50)]
        public string OperationType { get; set; }

        public string OperationDescription { get; set; }

        [StringLength(100)]
        public string AffectedEntity { get; set; }

        [StringLength(20)]
        public string OperationStatus { get; set; }

        [StringLength(50)]
        public string IpAddress { get; set; }

        [StringLength(50)]
        public string DeviceType { get; set; }

        // 导航属性
        [ForeignKey("StaffId")]
        public StaffInfo Staff { get; set; }
    }
}