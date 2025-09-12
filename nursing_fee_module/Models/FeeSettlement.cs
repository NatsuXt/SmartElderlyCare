using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace api.Models;

[Table("FeeSettlement")]
public class FeeSettlement
    {
        [Key]
        public int settlement_id { get; set; }
        public int elderly_id { get; set; }
        // 添加精度配置
        [Column(TypeName = "NUMBER(18, 2)")]
        public decimal total_amount { get; set; }
        // 添加精度配置
        [Column(TypeName = "NUMBER(18, 2)")]
        public decimal insurance_amount { get; set; }
        // 添加精度配置
        [Column(TypeName = "NUMBER(18, 2)")]
        public decimal personal_payment { get; set; }
        [Required]
        public DateTime billing_cycle_start { get; set; }
        [Required]
        public DateTime billing_cycle_end { get; set; }
        [Required]
        public DateTime created_at { get; set; }
        public DateTime settlement_date { get; set; }
        public string payment_status { get; set; } = string.Empty;
        public string payment_method { get; set; } = string.Empty;
        public int staff_id { get; set; }

        [ForeignKey("elderly_id")]
        public ElderlyInfo? ElderlyInfo { get; set; }

        [ForeignKey("staff_id")]
        public StaffInfo? StaffInfo { get; set; }

        public ICollection<NursingPlan>? NursingPlans { get; set; }
        public ICollection<FeeDetail>? FeeDetails { get; set; }
    }
