using System;
using System.Collections.Generic;

namespace api.Models.Dtos;

/// <summary>
/// 费用结算DTO，用于优化接口返回格式
/// </summary>
public class FeeSettlementDto
    {
        /// <summary>
        /// 结算单ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 老人ID
        /// </summary>
        public int elderly_id { get; set; }

        /// <summary>
        /// 账单周期开始日期
        /// </summary>
        public DateTime billing_cycle_start { get; set; }

        /// <summary>
        /// 账单周期结束日期
        /// </summary>
        public DateTime billing_cycle_end { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal total_amount { get; set; }

        /// <summary>
        /// 医保支付金额
        /// </summary>
        public decimal insurance_amount { get; set; }

        /// <summary>
        /// 个人支付金额
        /// </summary>
        public decimal personal_payment { get; set; }

        /// <summary>
        /// 支付状态
        /// </summary>
        public string payment_status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime created_at { get; set; }

        /// <summary>
        /// 费用明细
        /// </summary>
        public List<FeeDetailDto> fee_details { get; set; } = new List<FeeDetailDto>();
    }

/// <summary>
/// 费用明细DTO，包含各类费用的详细信息
/// </summary>
public class FeeDetailDto
{
    public string fee_type { get; set; } = string.Empty; // 费用类型：护理服务、药品、住宿、活动
    public string description { get; set; } = string.Empty; // 费用描述
    public decimal amount { get; set; } // 费用金额
    public DateTime? start_date { get; set; } // 开始日期（如适用）
    public DateTime? end_date { get; set; } // 结束日期（如适用）
    public int? quantity { get; set; } // 数量（如适用）
    public decimal? unit_price { get; set; } // 单价（如适用）
}