using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class MedicineInventory
{
    [Key]
    public int medicine_id { get; set; } // 药品ID 主键PK

    [Required(ErrorMessage = "药品名称不能为空")]
    [StringLength(100, ErrorMessage = "药品名称不能超过100个字符")]
    public string medicine_name { get; set; } = string.Empty; // 药品名称

    [Required(ErrorMessage = "药品类型不能为空")]
    [StringLength(50, ErrorMessage = "药品类型不能超过50个字符")]
    public string medicine_type { get; set; } = string.Empty; // 药品类型，如片剂、注射液等

    [Required(ErrorMessage = "单价不能为空")]
    public decimal unit_price { get; set; } // 单价

    [Required(ErrorMessage = "库存数量不能为空")]
    public int quantity_in_stock { get; set; } // 当前库存数量

    [Required(ErrorMessage = "最小库存预警值不能为空")]
    public int minimum_stock_level { get; set; } // 最小库存预警值

    [Required(ErrorMessage = "供应商信息不能为空")]
    [StringLength(100, ErrorMessage = "供应商名称不能超过100个字符")]
    public string supplier { get; set; } = string.Empty; // 供应商名称

    [Required(ErrorMessage = "过期日期不能为空")]
    public DateTime expiration_date { get; set; } // 过期日期

    [Column(TypeName = "CLOB")]
    public string? description { get; set; } // 药品描述，可选
}
