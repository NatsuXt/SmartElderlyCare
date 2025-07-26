using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCare.Models
{
    [Table("DisinfectionRecord")]
    public class DisinfectionRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DisinfectionId { get; set; }

        [StringLength(100)]
        public string Area { get; set; }

        public DateTime DisinfectionTime { get; set; }

        public int? StaffId { get; set; }

        [StringLength(50)]
        public string Method { get; set; }

        // 导航属性
        [ForeignKey("StaffId")]
        public StaffInfo Staff { get; set; }
    }
}