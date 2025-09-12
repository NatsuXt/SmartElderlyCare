using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

/// <summary>
/// 费用明细记录
/// </summary>
public class FeeDetail
{
    /// <summary>
    /// 费用明细ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    /// <summary>
    /// 关联的结算单ID
    /// </summary>
    [ForeignKey("FeeSettlement")]
    public int fee_settlement_id { get; set; }

    /// <summary>
    /// 费用类型
    /// </summary>
    [Required]
    [Column(TypeName = "NVARCHAR2(50)")]
    public string fee_type { get; set; }

    /// <summary>
    /// 费用描述
    /// </summary>
    [Required]
    [Column(TypeName = "NVARCHAR2(200)")]
    public string description { get; set; }

    /// <summary>
    /// 费用金额
    /// </summary>
    [Required]
    [Column(TypeName = "NUMBER(18,2)")]
    public decimal amount { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    [Column(TypeName = "DATE")]
    public DateTime? start_date { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    [Column(TypeName = "DATE")]
    public DateTime? end_date { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int? quantity { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    [Column(TypeName = "NUMBER(18,2)")]
    public decimal? unit_price { get; set; }

    /// <summary>
    /// 关联的结算单
    /// </summary>
    public FeeSettlement FeeSettlement { get; set; }
}