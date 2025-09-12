using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class MedicineProcurement
{
    [Key]
    public int procurement_id { get; set; } // 采购ID 主键PK

    public int medicine_id { get; set; } // 药品ID 外键，关联 MedicineInventory 表

    [Required(ErrorMessage = "采购数量不能为空")]
    public int purchase_quantity { get; set; } // 采购数量

    [Required(ErrorMessage = "采购时间不能为空")]
    public DateTime purchase_time { get; set; } // 采购时间

    public int staff_id { get; set; } // 经办人ID 外键，关联 StaffInfo 表

    [Required(ErrorMessage = "采购状态不能为空")]
    [StringLength(20, ErrorMessage = "状态不能超过20个字符")]
    public string status { get; set; } = string.Empty; // 状态，如待入库、已入库
}
