using System.Text.Json.Serialization;

namespace Staff_Info.DTOs
{
    // 员工信息DTO
    public class StaffInfoDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
        public decimal StaffId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string ContactPhone { get; set; }
        public string SkillLevel { get; set; }
        public string WorkSchedule { get; set; }
        
    }
    public class PostDeleteStaffInfoDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
        public decimal StaffId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Position { get; set; }
        public string ContactPhone { get; set; }
        public string SkillLevel { get; set; }
        public string WorkSchedule { get; set; }
        public string Email { get; set; }
        public DateTime? HireDate { get; set; }
        public decimal? Salary { get; set; }
        
    }
    
    // 护理计划DTO
    public class NursingPlanDto
    {
        public decimal PlanId { get; set; }
        public decimal ElderlyId { get; set; }
        public decimal? StaffId { get; set; } // 排班后分配
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public string CareType { get; set; }
        public string Priority { get; set; }
        public string EvaluationStatus { get; set; }
    }
    public class PostNursingPlanDto
    {
        //public decimal PlanId { get; set; }
        public decimal ElderlyId { get; set; }
        public decimal? StaffId { get; set; } // 排班后分配
        public DateTime PlanStartDate { get; set; }
        public DateTime PlanEndDate { get; set; }
        public string CareType { get; set; }
        public string Priority { get; set; }
        public string EvaluationStatus { get; set; }
    }
    // 智能排班请求DTO
    public class NursingScheduleRequestDto
    {
        // 可以保留空结构，或者完全移除这个DTO
        // 因为现在是一键全排，不需要参数
    }

    // 活动安排DTO
    public class ActivityScheduleDto
    {
        public string ActivityName { get; set; }
        public DateTime ActivityDate { get; set; }
        public TimeSpan ActivityTime { get; set; }
        public string Location { get; set; }
        public decimal StaffId { get; set; }
        public string ElderlyParticipants { get; set; }
    }

    // 医嘱管理DTO
    public class MedicalOrderDto
    {
        public decimal ElderlyId { get; set; }
        public decimal StaffId { get; set; }
        public decimal MedicineId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
    }

    // 操作日志DTO
    public class OperationLogDto
    {
        public decimal StaffId { get; set; }
        public string OperationType { get; set; }
        public string OperationDescription { get; set; }
        public string AffectedEntity { get; set; }
        public string DeviceType { get; set; }
    }
    //get sosdto
    public class GetEmergencyInfoDto
    {
        public decimal CALL_ID { get; set; }  
        public decimal ELDERLY_ID { get; set; }
    
        public DateTime CALL_TIME { get; set; }
        public string CALL_TYPE { get; set; }
        public decimal? ROOM_ID { get; set; }
        public DateTime? RESPONSE_TIME { get; set; }
        public decimal? RESPONSE_STAFF_ID { get; set; }
        public bool FOLLOW_UP_REQUIRED { get; set; }
        public string CALL_STATUS { get; set; }
        public string HANDLING_RESULT { get; set; }
    }
    // 紧急SOS创建DTO
    public class EmergencySOSCreateDto
    {
        public decimal ElderlyId { get; set; }
        public string CallType { get; set; }
        //public decimal? RoomId { get; set; }
    }

// SOS响应接受DTO
    public class EmergencySOSAcceptDto
    {
        public decimal CallId { get; set; }
        public decimal ResponseStaffId { get; set; }
    }

// SOS处理完成DTO
    public class EmergencySOSCompleteDto
    {
        public decimal CallId { get; set; }
        public decimal StaffId { get; set; }
        public string HandlingResult { get; set; }
    }
    public class SOSNotificationDto
    {
        public decimal CallId { get; set; }
        public decimal ElderlyId { get; set; }
        public string ElderlyName { get; set; }
        public string CallType { get; set; }
        public DateTime CallTime { get; set; }
        public string RoomNumber { get; set; }
        public decimal Floor { get; set; }
    }


    // 系统公告DTO
    public class SystemAnnouncementDto
    {
        public DateTime AnnouncementDate { get; set; }
        public string AnnouncementType { get; set; }
        public string AnnouncementContent { get; set; }
        public string Audience { get; set; }
        public decimal CreatedBy { get; set; }
    }

    // 消毒记录DTO
    public class DisinfectionRecordDto
    {
        public decimal DisinfectionId { get; set; }
        public string Area { get; set; }
        public DateTime DisinfectionTime { get; set; }
        public decimal StaffId { get; set; }
        public string Methods { get; set; }
    }

    // 消毒报告请求DTO
    public class DisinfectionReportRequestDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }
}