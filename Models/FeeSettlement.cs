using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("FEESETTLEMENT")]
    public class FeeSettlement
    {
        [Key]
        [Column("SETTLEMENT_ID")]
        public int SettlementId { get; set; }

        [ForeignKey(nameof(Elderly))]
        [Column("ELDERLY_ID")]
        public int ElderlyId { get; set; }

        [Column("TOTAL_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column("INSURANCE_AMOUNT", TypeName = "decimal(18,2)")]
        public decimal InsuranceAmount { get; set; }

        [Column("PERSONAL_PAYMENT", TypeName = "decimal(18,2)")]
        public decimal PersonalPayment { get; set; }

        [Column("SETTLEMENT_DATE")]
        public DateTime SettlementDate { get; set; }

        [Column("PAYMENT_STATUS"), MaxLength(50)]
        public string PaymentStatus { get; set; }

        [Column("PAYMENT_METHOD"), MaxLength(50)]
        public string PaymentMethod { get; set; }

        [Column("STAFF_ID")]
        public int StaffId { get; set; }

        public ElderlyInfo Elderly { get; set; }

        public ICollection<FeeDetail> FeeDetails { get; set; }

    }
}
