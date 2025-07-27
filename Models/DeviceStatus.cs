using System;
using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 设备状态实体类
    /// </summary>
    public class DeviceStatus
    {
        /// <summary>
        /// 设备ID（主键）
        /// </summary>
        [Key]
        public int DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string? DeviceName { get; set; }

        /// <summary>
        /// 设备类型（如：心率监测器、血压计、跌倒检测器、摄像头等）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string? DeviceType { get; set; }

        /// <summary>
        /// 安装日期
        /// </summary>
        [Required]
        public DateTime InstallationDate { get; set; }

        /// <summary>
        /// 设备状态（如：正常、故障、维护中、停用）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string? Status { get; set; }

        /// <summary>
        /// 最后维护日期
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// 设备维护状态
        /// </summary>
        [StringLength(20)]
        public string? MaintenanceStatus { get; set; }

        /// <summary>
        /// 设备所在位置
        /// </summary>
        [Required]
        [StringLength(100)]
        public string? Location { get; set; }

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
