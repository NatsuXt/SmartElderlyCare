using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public class FeeDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    [ForeignKey("FeeSettlement")]
    public int fee_settlement_id { get; set; }

    [Required]
    [Column(TypeName = "NVARCHAR2(50)")]
    public string fee_type { get; set; }

    [Required]
    [Column(TypeName = "NVARCHAR2(200)")]
    public string description { get; set; }

    [Required]
    [Column(TypeName = "NUMBER(18,2)")]
    public decimal amount { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? start_date { get; set; }

    [Column(TypeName = "DATE")]
    public DateTime? end_date { get; set; }

    public int? quantity { get; set; }

    [Column(TypeName = "NUMBER(18,2)")]
    public decimal? unit_price { get; set; }

    public FeeSettlement FeeSettlement { get; set; }
}