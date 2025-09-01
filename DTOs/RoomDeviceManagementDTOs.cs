using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.DTOs
{
    // ======== 房间管理 DTOs ========
    public class RoomCreateDto
    {
        [Required]
        [MaxLength(20)]
        public string RoomNumber { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string RoomType { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 10)]
        public int Capacity { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;
        
        [Required]
        [Range(0, 99999)]
        public decimal Rate { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string BedType { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 50)]
        public int Floor { get; set; }
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
        
        [Range(0, 99999)]
        public decimal? Rate { get; set; }
        
        [MaxLength(50)]
        public string? BedType { get; set; }
        
        [Range(1, 50)]
        public int? Floor { get; set; }
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
        public decimal Rate { get; set; }
        public string BedType { get; set; }
        public int Floor { get; set; }
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

    // ======== 房间入住管理 DTOs ========
    
    /// <summary>
    /// 入住登记 DTO
    /// </summary>
    public class CheckInDto
    {
        [Required]
        public decimal ElderlyId { get; set; }
        
        [Required]
        public int RoomId { get; set; }
        
        public DateTime CheckInDate { get; set; } = DateTime.Now;
        
        [MaxLength(10)]
        public string? BedNumber { get; set; }
        
        [MaxLength(200)]
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// 退房登记 DTO
    /// </summary>
    public class CheckOutDto
    {
        [Required]
        public int OccupancyId { get; set; }
        
        public DateTime CheckOutDate { get; set; } = DateTime.Now;
        
        [MaxLength(200)]
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// 入住记录详情 DTO
    /// </summary>
    public class OccupancyRecordDto
    {
        public int OccupancyId { get; set; }
        public int RoomId { get; set; }
        public decimal ElderlyId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string ElderlyName { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? BedNumber { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    /// <summary>
    /// 生成账单请求 DTO
    /// </summary>
    public class GenerateBillDto
    {
        public decimal? ElderlyId { get; set; } // 如果为空则生成所有账单
        
        [Required]
        public DateTime BillingStartDate { get; set; }
        
        [Required]
        public DateTime BillingEndDate { get; set; }
        
        [Range(0.01, 999999.99)]
        public decimal? DailyRate { get; set; } // 可以覆盖房间默认费率
        
        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// 账单记录 DTO
    /// </summary>
    public class BillingRecordDto
    {
        public int BillingId { get; set; }
        public int OccupancyId { get; set; }
        public decimal ElderlyId { get; set; }
        public string ElderlyName { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime BillingStartDate { get; set; }
        public DateTime BillingEndDate { get; set; }
        public int Days { get; set; }
        public decimal DailyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal PaidAmount { get; set; }
        public decimal UnpaidAmount { get; set; }
        public DateTime BillingDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    /// <summary>
    /// 支付账单 DTO
    /// </summary>
    public class PaymentDto
    {
        [Required]
        public int BillingId { get; set; }
        
        [Required]
        [Range(0.01, 999999.99)]
        public decimal PaymentAmount { get; set; }
        
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        [MaxLength(500)]
        public string? PaymentRemarks { get; set; }
    }

    /// <summary>
    /// 房间入住统计 DTO
    /// </summary>
    public class RoomOccupancyStatsDto
    {
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int MaintenanceRooms { get; set; }
        public decimal OccupancyRate { get; set; }
        public DateTime StatDate { get; set; }
    }

}
