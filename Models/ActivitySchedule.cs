// ActivitySchedule.cs
using System.Collections.Generic;

namespace ElderlyCareManagement.Models
{
    public class ActivitySchedule
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public DateTime ActivityDate { get; set; }
        public TimeSpan ActivityTime { get; set; }
        public string Location { get; set; }
        public int? StaffId { get; set; }
        public string ElderlyParticipants { get; set; }
        public string ActivityDescription { get; set; }
        
        // Navigation properties
        public StaffInfo Staff { get; set; }
        //public ICollection<ActivityParticipation> ActivityParticipations { get; set; }
    }
}