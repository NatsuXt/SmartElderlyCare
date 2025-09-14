using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace api.Models;

[Table("FeeSettlement")]
public class FeeSettlement
{
    [Key]
    [Column("SETTLEMENT_ID")]
    public int settlement_id { get; set; }
    [Column("ELDERLY_ID")]
    public int elderly_id { get; set; }
    [Column("TOTAL_AMOUNT", TypeName = "NUMBER(18, 2)")]
    public decimal total_amount { get; set; }
    [Column("INSURANCE_AMOUNT", TypeName = "NUMBER(18, 2)")]
    public decimal insurance_amount { get; set; }
    [Column("PERSONAL_PAYMENT", TypeName = "NUMBER(18, 2)")]
    public decimal personal_payment { get; set; }
    [Column("SETTLEMENT_DATE")]
    public DateTime settlement_date { get; set; }
    [Column("PAYMENT_STATUS")]
    public string payment_status { get; set; } = string.Empty;
    [Column("PAYMENT_METHOD")]
    public string payment_method { get; set; } = string.Empty;
    [Column("STAFF_ID")]
    public int staff_id { get; set; }

    // 注意：移除了导航属性，避免复杂的依赖关系
    // 在当前模块中，我们通过外键ID来引用相关实体
}