using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.DTOs
{
    // ======== 房间管理 DTOs ========
    public class RoomCreateDto
    {
        [Required]
        [MaxLength(20)]
        public string RoomNumber { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string RoomType { get; set; }
        
        [Required]
        [Range(1, 10)]
        public int Capacity { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class RoomUpdateDto
    {
        [MaxLength(20)]
        public string? RoomNumber { get; set; }
        
        [MaxLength(50)]
        public string? RoomType { get; set; }
        
        [Range(1, 10)]
        public int? Capacity { get; set; }
        
        [MaxLength(20)]
        public string? Status { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
    }

    // ======== 设备状态 DTOs ========
    public class DeviceCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string DeviceName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string DeviceType { get; set; }
        
        [Required]
        public DateTime InstallationDate { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
        
        public int? RoomId { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(200)]
        public string? Location { get; set; }
        
        public DateTime? LastMaintenanceDate { get; set; }
    }

    public class DeviceUpdateDto
    {
        [MaxLength(100)]
        public string? DeviceName { get; set; }
        
        [MaxLength(50)]
        public string? DeviceType { get; set; }
        
        public DateTime? InstallationDate { get; set; }
        
        [MaxLength(20)]
        public string? Status { get; set; }
        
        public int? RoomId { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(200)]
        public string? Location { get; set; }
        
        public DateTime? LastMaintenanceDate { get; set; }
    }

    // ======== 电子围栏 DTOs ========
    public class FenceCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string FenceName { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string AreaDefinition { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
    }

    public class FenceUpdateDto
    {
        [MaxLength(100)]
        public string? FenceName { get; set; }
        
        [MaxLength(500)]
        public string? AreaDefinition { get; set; }
        
        [MaxLength(20)]
        public string? Status { get; set; }
    }

    // ======== 围栏日志 DTOs ========
    public class FenceLogCreateDto
    {
        [Required]
        public int ElderlyId { get; set; }
        
        [Required]
        public int FenceId { get; set; }
        
        [Required]
        public DateTime EntryTime { get; set; }
        
        public DateTime? ExitTime { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string EventType { get; set; }
        
        public DateTime? EventTime { get; set; }
        
        [MaxLength(200)]
        public string? Location { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class FenceLogUpdateDto
    {
        public int? ElderlyId { get; set; }
        public int? FenceId { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }

    // ======== 健康监测 DTOs ========
    public class HealthDataCreateDto
    {
        [Required]
        public int ElderlyId { get; set; }
        
        public DateTime? MonitoringDate { get; set; }
        
        [Range(40, 200)]
        public int? HeartRate { get; set; }
        
        [Range(80, 200)]
        public int? BloodPressureHigh { get; set; }
        
        [Range(40, 120)]
        public int? BloodPressureLow { get; set; }
        
        [Range(2.0, 30.0)]
        public decimal? BloodSugar { get; set; }
        
        [Range(35.0, 42.0)]
        public decimal? BodyTemperature { get; set; }
        
        [Range(0, 10)]
        public int? ActivityLevel { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    public class HealthDataUpdateDto
    {
        public int? ElderlyId { get; set; }
        public DateTime? MonitoringDate { get; set; }
        
        [Range(40, 200)]
        public int? HeartRate { get; set; }
        
        [MaxLength(20)]
        public string? BloodPressure { get; set; }
        
        [Range(70, 100)]
        public decimal? OxygenLevel { get; set; }
        
        [Range(30, 45)]
        public decimal? Temperature { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    // ======== 查询结果 DTOs ========
    public class RoomDetailDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DeviceDetailDto>? Devices { get; set; }
    }

    public class DeviceDetailDto
    {
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public DateTime InstallationDate { get; set; }
        public string Status { get; set; }
        public int? RoomId { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class FenceDetailDto
    {
        public int FenceId { get; set; }
        public string AreaDefinition { get; set; }
    }

    public class FenceLogDetailDto
    {
        public int EventLogId { get; set; }
        public int ElderlyId { get; set; }
        public int FenceId { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }

    public class HealthDataDetailDto
    {
        public int MonitoringId { get; set; }
        public int ElderlyId { get; set; }
        public DateTime MonitoringDate { get; set; }
        public int? HeartRate { get; set; }
        public string? BloodPressure { get; set; }
        public decimal? OxygenLevel { get; set; }
        public decimal? Temperature { get; set; }
        public string? Notes { get; set; }
    }

    // ======== 通用响应 DTOs ========
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public int? TotalCount { get; set; }
    }

    public class PagedRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool SortDesc { get; set; } = false;
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    // ======== Info别名DTOs (为了兼容服务层) ========
    public class HealthMonitoringInfoDTO : HealthDataDetailDto
    {
    }

    public class CreateHealthMonitoringDTO : HealthDataCreateDto
    {
    }

    public class FenceInfoDTO : FenceDetailDto
    {
    }

    public class FenceLogInfoDTO : FenceLogDetailDto
    {
    }

}
