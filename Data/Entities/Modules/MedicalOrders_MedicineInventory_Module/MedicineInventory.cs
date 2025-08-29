using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    public class MedicineInventory
    {
        [Key]
        public int medicine_id { get; set; } // 药品ID 主键PK

        [Required, StringLength(100)]
        public string medicine_name { get; set; } = string.Empty; // 药品名称

        [StringLength(100)]
        public string? specification { get; set; } // 规格（如0.25g×24片/盒）

        [StringLength(50)]
        public string? dosage_form { get; set; } // 剂型（片剂/胶囊/注射液等）

        [StringLength(20)]
        public string? unit { get; set; } // 计量单位（盒/瓶/片等）作为开药/计价基础单位

        [StringLength(50)]
        public string? category { get; set; } // 药品类别（处方药/OTC/中成药/西药等）

        [Column(TypeName = "CLOB")]
        public string? description { get; set; } // 药品说明 可空
    }
}
