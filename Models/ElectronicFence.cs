using System;
using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 电子围栏信息实体类
    /// </summary>
    public class ElectronicFence
    {
        /// <summary>
        /// 围栏ID（主键）
        /// </summary>
        [Key]
        public int FenceId { get; set; }

        /// <summary>
        /// 区域范围定义，如坐标点集合
        /// </summary>
        [Required]
        public string? AreaDefinition { get; set; }

        /// <summary>
        /// 围栏名称
        /// </summary>
        [StringLength(100)]
        public string? FenceName { get; set; }

        /// <summary>
        /// 围栏类型
        /// </summary>
        [StringLength(50)]
        public string? FenceType { get; set; }

        /// <summary>
        /// 围栏状态（如：启用、禁用、维护中）
        /// </summary>
        [StringLength(20)]
        public string? Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}
