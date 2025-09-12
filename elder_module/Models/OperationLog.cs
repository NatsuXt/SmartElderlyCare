using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    public class OperationLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public int StaffId { get; set; }

        public DateTime OperationTime { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string OperationType { get; set; }

        public string OperationDescription { get; set; }

        [MaxLength(100)]
        public string AffectedEntity { get; set; }

        [MaxLength(20)]
        public string OperationStatus { get; set; }

        [MaxLength(50)]
        public string IpAddress { get; set; }

        [MaxLength(50)]
        public string DeviceType { get; set; }

        
    }
}
