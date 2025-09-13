using System;

namespace ElderCare.Api.Modules.Medical
{
    /// <summary>
    /// 对应表 MEDICINE_DISPENSE
    /// </summary>
    public sealed class DispenseRecord
    {
        public int Dispense_Id { get; set; }
        public string Bill_Id { get; set; } = string.Empty;

        public int Elderly_Id { get; set; }
        public int? Order_Id { get; set; }
        public int? Staff_Id { get; set; }

        public int Medicine_Id { get; set; }
        public int Stock_Batch_Id { get; set; }
        public int Quantity { get; set; }

        public decimal Unit_Sale_Price { get; set; }
        public decimal Total_Amount { get; set; }

        public string Payment_Status { get; set; } = "UNPAID";
        public string? Payment_Method { get; set; }
        public int? Settlement_Id { get; set; }
        public DateTime? Dispense_Time { get; set; }

        // 业务状态（RESERVED / DISPENSED / CANCELLED ...）
        public string Status { get; set; } = "RESERVED";

        public string? Remarks { get; set; }
    }
}
