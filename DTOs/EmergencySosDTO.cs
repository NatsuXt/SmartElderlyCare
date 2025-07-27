// DTOs/EmergencySosDTO.cs
namespace ElderlyCareManagement.DTOs
{
    public class EmergencySosDTO
    {
        public int CallId { get; set; }
        public int ElderlyId { get; set; }
        public DateTime CallTime { get; set; }
        public string CallType { get; set; }
        public string CallStatus { get; set; }
    }

    public class EmergencySosCreateDTO
    {
        public int ElderlyId { get; set; }
        public string CallType { get; set; }
        public int? RoomId { get; set; }
    }

    public class EmergencySosDetailDTO : EmergencySosDTO
    {
        public int? RoomId { get; set; }
        public DateTime? ResponseTime { get; set; }
        public int? ResponseStaffId { get; set; }
        public string ResponseStaffName { get; set; }
        public bool FollowUpRequired { get; set; }
        public string HandlingResult { get; set; }
    }

    public class EmergencySosUpdateDTO
    {
        public int CallId { get; set; }
        public int? ResponseStaffId { get; set; }
        public string HandlingResult { get; set; }
        public string CallStatus { get; set; }
    }
}