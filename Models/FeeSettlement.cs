using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("FEESETTLEMENTS")]
public class FeeSettlement
{
    [Key]
    [Column("SETTLEMENT_ID")]
    public int settlement_id { get; set; } // 结算ID 主键PK

    [Column("ELDERLY_ID")]
    public int elderly_id { get; set; } // 老人ID 外键

    [Column("TOTAL_AMOUNT")]
    public decimal total_amount { get; set; } // 结算总金额

    [Column("INSURANCE_AMOUNT")]
    public decimal insurance_amount { get; set; } // 保险支付金额

    [Column("PERSONAL_PAYMENT")]
    public decimal personal_payment { get; set; } // 个人支付金额

    [Column("SETTLEMENT_DATE")]
    public DateTime settlement_date { get; set; } // 结算日期

    [Column("PAYMENT_STATUS")]
    public string payment_status { get; set; } = string.Empty; // 支付状态 如已支付、待支付

    [Column("PAYMENT_METHOD")]
    public string payment_method { get; set; } = string.Empty; // 支付方式 如现金、信用卡

    [Column("STAFF_ID")]
    public int staff_id { get; set; } // 员工ID 外键，处理结算的员工ID

    // 导航属性
    public virtual ElderlyInfo? ElderlyInfo { get; set; }
    public virtual StaffInfo? StaffInfo { get; set; }
}