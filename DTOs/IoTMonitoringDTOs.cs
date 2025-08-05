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
}
