// SystemAnnouncement.cs
namespace ElderlyCareManagement.Models
{
    public class SystemAnnouncement
    {
        public int AnnouncementId { get; set; }
        public DateTime AnnouncementDate { get; set; }
        public string AnnouncementType { get; set; }
        public string AnnouncementContent { get; set; }
        public string Status { get; set; }
        public string Audience { get; set; }
        public int CreatedBy { get; set; }
        public string ReadStatus { get; set; }
        public string Comments { get; set; }
        
        // Navigation property
        public StaffInfo Staff { get; set; }
    }
}