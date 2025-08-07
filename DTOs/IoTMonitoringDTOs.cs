using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.DTOs
{
    /// <summary>
    /// 设备故障上报数据传输对象
    /// </summary>
    public class DeviceFaultReportDto
    {
        [Required]
        public int DeviceId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string DeviceType { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string FaultStatus { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? FaultDescription { get; set; }
        
        public DateTime ReportTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// GPS位置上报数据传输对象
    /// </summary>
    public class GpsLocationReportDto
    {
        [Required]
        public int ElderlyId { get; set; }
        
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
        
        public DateTime LocationTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 健康数据上报数据传输对象
    /// </summary>
    public class HealthDataReportDto
    {
        [Required]
        public int ElderlyId { get; set; }
        
        public int? HeartRate { get; set; }
        
        [MaxLength(20)]
        public string? BloodPressure { get; set; }
        
        public float? OxygenLevel { get; set; }
        
        public float? Temperature { get; set; }
        
        public DateTime MeasurementTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 设备状态上报数据传输对象
    /// </summary>
    public class DeviceStatusReportDto
    {
        [Required]
        public int DeviceId { get; set; }
        
        [MaxLength(100)]
        public string? DeviceName { get; set; }
        
        [MaxLength(50)]
        public string? DeviceType { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Location { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        public DateTime ReportTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 围栏配置数据传输对象
    /// </summary>
    public class FenceConfigDto
    {
        public int? FenceId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string FenceName { get; set; } = string.Empty;
        
        [Required]
        public string AreaDefinition { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? FenceType { get; set; }
        
        [MaxLength(20)]
        public string Status { get; set; } = "Enabled";
    }

    /// <summary>
    /// 护理人员位置更新数据传输对象
    /// </summary>
    public class StaffLocationUpdateDto
    {
        [Required]
        public int StaffId { get; set; }
        
        public int? Floor { get; set; }
        
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 围栏检查测试数据传输对象
    /// </summary>
    public class FenceTestDto
    {
        [Required]
        public double Latitude { get; set; }
        
        [Required]
        public double Longitude { get; set; }
    }
}
