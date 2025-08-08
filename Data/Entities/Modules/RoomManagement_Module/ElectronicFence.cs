using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 电子围栏信息实体类
    /// </summary>
    [Table("ElectronicFence")]
    public class ElectronicFence
    {
        /// <summary>
        /// 围栏ID（主键，自增）
        /// </summary>
        [Key]
        [Column("fence_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FenceId { get; set; }

        /// <summary>
        /// 区域范围定义，如坐标点集合
        /// </summary>
        [Required]
        [Column("area_definition", TypeName = "TEXT")]
        public string AreaDefinition { get; set; } = string.Empty;
    }
}
