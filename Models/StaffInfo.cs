using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCare.Models
{
    [Table("StaffInfo")]
    public class StaffInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StaffId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(50)]
        public string Position { get; set; }

        [StringLength(20)]
        public string ContactPhone { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Salary { get; set; }

        // 导航属性
        public ICollection<NursingPlan> NursingPlans { get; set; }
        public ICollection<ActivitySchedule> ActivitySchedules { get; set; }
        public ICollection<MedicalOrder> MedicalOrders { get; set; }
        public ICollection<OperationLog> OperationLogs { get; set; }
        public ICollection<EmergencySOS> EmergencySOSResponses { get; set; }
        public ICollection<SystemAnnouncement> SystemAnnouncements { get; set; }
        public ICollection<DisinfectionRecord> DisinfectionRecords { get; set; }
    }
}