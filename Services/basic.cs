using ElderlyCare.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public interface IStaffService
    {
        Task<StaffInfo> GetStaffByIdAsync(int staffId);
        Task<List<StaffInfo>> GetAllStaffAsync();
        Task AddStaffAsync(StaffInfo staff);
        Task UpdateStaffAsync(StaffInfo staff);
        Task DeleteStaffAsync(int staffId);
        Task<List<StaffInfo>> GetStaffByPositionAsync(string position);
    }

    public interface INursingPlanService
    {
        Task<List<NursingPlan>> GetAllNursingPlansAsync();
        Task<NursingPlan> GetNursingPlanByIdAsync(int planId);
        Task AssignNursingPlanAsync(int planId, int staffId);
        Task<List<NursingPlan>> GetUnassignedNursingPlansAsync();
        Task<List<NursingPlan>> GetNursingPlansByStaffIdAsync(int staffId);
        Task CreateNursingPlanAsync(NursingPlan plan);
        Task UpdateNursingPlanAsync(NursingPlan plan);
        Task DeleteNursingPlanAsync(int planId);
    }

    public interface IActivityScheduleService
    {
        Task<List<ActivitySchedule>> GetAllActivitiesAsync();
        Task<ActivitySchedule> GetActivityByIdAsync(int activityId);
        Task CreateActivityAsync(ActivitySchedule activity);
        Task UpdateActivityAsync(ActivitySchedule activity);
        Task DeleteActivityAsync(int activityId);
        Task<List<ActivitySchedule>> GetActivitiesByStaffIdAsync(int staffId);
        Task<List<ActivitySchedule>> GetUpcomingActivitiesAsync(int days);
    }

    public interface IMedicalOrderService
    {
        Task<List<MedicalOrder>> GetAllMedicalOrdersAsync();
        Task<MedicalOrder> GetMedicalOrderByIdAsync(int orderId);
        Task CreateMedicalOrderAsync(MedicalOrder order);
        Task UpdateMedicalOrderAsync(MedicalOrder order);
        Task DeleteMedicalOrderAsync(int orderId);
        Task<List<MedicalOrder>> GetMedicalOrdersByStaffIdAsync(int staffId);
        Task<List<MedicalOrder>> GetMedicalOrdersByElderlyIdAsync(int elderlyId);
    }

    public interface IOperationLogService
    {
        Task LogOperationAsync(OperationLog log);
        Task<List<OperationLog>> GetOperationLogsAsync(int? staffId = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<OperationLog>> GetRecentOperationLogsAsync(int count);
    }

    public interface IEmergencySOSService
    {
        Task<EmergencySOS> CreateEmergencyCallAsync(EmergencySOS emergencyCall);
        Task RespondToEmergencyCallAsync(int callId, int staffId);
        Task CompleteEmergencyCallAsync(int callId, string handlingResult);
        Task<List<EmergencySOS>> GetPendingEmergencyCallsAsync();
        Task<List<EmergencySOS>> GetEmergencyCallsByStaffIdAsync(int staffId);
        Task<List<EmergencySOS>> GetEmergencyCallsByElderlyIdAsync(int elderlyId);
    }

    public interface ISystemAnnouncementService
    {
        Task CreateAnnouncementAsync(SystemAnnouncement announcement);
        Task UpdateAnnouncementAsync(SystemAnnouncement announcement);
        Task DeleteAnnouncementAsync(int announcementId);
        Task<SystemAnnouncement> GetAnnouncementByIdAsync(int announcementId);
        Task<List<SystemAnnouncement>> GetAnnouncementsByTypeAsync(string type);
        Task<List<SystemAnnouncement>> GetActiveAnnouncementsAsync();
        Task<List<SystemAnnouncement>> GetAnnouncementsByCreatorAsync(int staffId);
    }

    public interface IDisinfectionRecordService
    {
        Task CreateDisinfectionRecordAsync(DisinfectionRecord record);
        Task<List<DisinfectionRecord>> GetDisinfectionRecordsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<DisinfectionRecord>> GetDisinfectionRecordsByStaffIdAsync(int staffId);
        Task<DisinfectionRecord> GetDisinfectionRecordByIdAsync(int recordId);
        Task DeleteDisinfectionRecordAsync(int recordId);
        Task GenerateMonthlyDisinfectionReportAsync(int year, int month);
    }

    public interface INursingSchedulerService
    {
        Task AutoAssignNursingPlansAsync();
    }
}