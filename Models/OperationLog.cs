// OperationLog.cs
namespace ElderlyCareManagement.Models
{
    public class OperationLog
    {
        public int LogId { get; set; }
        public int StaffId { get; set; }
        public DateTime OperationTime { get; set; }
        public string OperationType { get; set; }
        public string OperationDescription { get; set; }
        public string AffectedEntity { get; set; }
        public string OperationStatus { get; set; }
        public string IpAddress { get; set; }
        public string DeviceType { get; set; }
        
        // Navigation property
        public StaffInfo Staff { get; set; }
    }
}