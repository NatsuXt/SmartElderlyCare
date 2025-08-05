using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class MedicalOrder
    {
        [Key]
        public int order_id { get; set; } // 医嘱ID 主键PK

        public int elderly_id { get; set; } // 老人ID 外键 关联 ElderlyInfo 表

        public int staff_id { get; set; } // 员工ID 外键 关联 StaffInfo 表

        public int medicine_id { get; set; } // 药品ID 外键 关联 MedicineInventory 表

        [Column(TypeName = "DATE")]
        public DateTime order_date { get; set; } // 开立日期

        [StringLength(20)]
        public string dosage { get; set; } = string.Empty; // 剂量

        [StringLength(50)]
        public string frequency { get; set; } = string.Empty; // 频次

        [StringLength(50)]
        public string duration { get; set; } = string.Empty; // 持续时间

        [ForeignKey("elderly_id")]
        public ElderlyInfo? Elderly { get; set; } // 导航属性：关联老人

        [ForeignKey("staff_id")]
        public StaffInfo? Staff { get; set; } // 导航属性：关联员工

        [ForeignKey("medicine_id")]
        public MedicineInventory? Medicine { get; set; } // 导航属性：关联药品
    }
}
