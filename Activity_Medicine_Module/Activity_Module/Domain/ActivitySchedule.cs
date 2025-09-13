namespace ElderCare.Api.Modules.Activities
{
    public sealed class ActivitySchedule
    {
        public int activity_id { get; set; }
        public string activity_name { get; set; } = string.Empty;
        public DateTime activity_date { get; set; }
        public string activity_time { get; set; } = string.Empty; 
        public string location { get; set; } = string.Empty;
        public int staff_id { get; set; }
        public string? elderly_participants { get; set; }
        public string? activity_description { get; set; }
        public string status { get; set; } = string.Empty;
    }
}
