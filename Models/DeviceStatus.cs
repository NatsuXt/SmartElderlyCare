using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 设备状态实体类
    /// </summary>
    [Table("DeviceStatus")]
    public class DeviceStatus
    {
        /// <summary>
        /// 设备ID（主键，自增）
        /// </summary>
        [Key]
        [Column("device_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        [Required]
        [Column("device_name")]
        [MaxLength(100)]
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// 设备类型（如：智能床垫、心率监测仪等）
        /// </summary>
        [Required]
        [Column("device_type")]
        [MaxLength(50)]
        public string DeviceType { get; set; } = string.Empty;

        /// <summary>
        /// 安装日期
        /// </summary>
        [Required]
        [Column("installation_date")]
        public DateTime InstallationDate { get; set; }

        /// <summary>
        /// 设备状态（如：正常、故障）
        /// </summary>
        [Required]
        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 最后维护日期
        /// </summary>
        [Column("last_maintenance_date")]
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// 设备维护状态
        /// </summary>
        [Column("maintenance_status")]
        [MaxLength(20)]
        public string? MaintenanceStatus { get; set; }

        /// <summary>
        /// 设备所在位置
        /// </summary>
        [Required]
        [Column("location")]
        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;
    }
}
