using System;

namespace ElderCare.Api.Modules.Medical
{
    /// <summary>创建一个新的入库批次</summary>
    public sealed class CreateStockBatchDto
    {
        public int medicine_id { get; set; }
        public string? batch_no { get; set; }                    // 可选
        public DateTime expiration_date { get; set; }            // 推荐前端传 YYYY-MM-DD
        public decimal cost_price { get; set; }                  // 进价
        public decimal sale_price { get; set; }                  // 售价
        public int quantity_in_stock { get; set; }               // 入库数量
        public int minimum_stock_level { get; set; }             // 最低库存告警线
        public string? location { get; set; }                    // 货位
        public string? supplier { get; set; }                    // 供应商
        public int is_active { get; set; } = 1;                  // 1=可用, 0=停用
    }

    /// <summary>更新批次（PUT，覆盖式更新）</summary>
    public sealed class UpdateStockBatchDto
    {
        public string? batch_no { get; set; }
        public DateTime expiration_date { get; set; }
        public decimal cost_price { get; set; }
        public decimal sale_price { get; set; }
        public int quantity_in_stock { get; set; }
        public int minimum_stock_level { get; set; }
        public string? location { get; set; }
        public string? supplier { get; set; }
        public int is_active { get; set; } = 1;
    }

    /// <summary>增减库存（PATCH /quantity）</summary>
    public sealed class AdjustQuantityDto
    {
        /// <summary>正数=入库；负数=出库</summary>
        public int delta { get; set; }
    }

    /// <summary>批次查询返回模型</summary>
    public sealed class StockBatch
    {
        public int stock_batch_id { get; set; }
        public int medicine_id { get; set; }
        public string? batch_no { get; set; }
        public DateTime expiration_date { get; set; }
        public decimal cost_price { get; set; }
        public decimal sale_price { get; set; }
        public int quantity_in_stock { get; set; }
        public int reserved_quantity { get; set; }
        public int minimum_stock_level { get; set; }
        public string? location { get; set; }
        public string? supplier { get; set; }
        public int is_active { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
