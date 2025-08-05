// StaffInfo.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElderlyCareManagement.Models
{
    public class StaffInfo
    {
        [Key]public int StaffId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Position { get; set; }
        public string ContactPhone { get; set; }
        public string Email { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public string SkillLevel { get; set; }
        public string WorkSchedule { get; set; }
        
        // Navigation properties
        public ICollection<NursingPlan> NursingPlans { get; set; }
        public ICollection<ActivitySchedule> ActivitySchedules { get; set; }
        public ICollection<MedicalOrder> MedicalOrders { get; set; }
        public ICollection<OperationLog> OperationLogs { get; set; }
        public ICollection<EmergencySOS> EmergencySosResponses { get; set; }
        public ICollection<SystemAnnouncement> SystemAnnouncements { get; set; }
        public ICollection<DisinfectionRecord> DisinfectionRecords { get; set; }
        public virtual ICollection<StaffSchedule> Schedules { get; set; }
        public virtual ICollection<StaffLocation> Locations { get; set; }
        public virtual ICollection<SosNotification> Notifications { get; set; }
    }
}