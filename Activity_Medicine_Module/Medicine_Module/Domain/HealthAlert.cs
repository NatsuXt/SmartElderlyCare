namespace ElderCare.Api.Modules.Medical
{
    public sealed class HealthAlert
    {
        public int alert_id { get; set; }
        public int elderly_id { get; set; }
        public string alert_type { get; set; } = string.Empty;
        public DateTime alert_time { get; set; }
        public string alert_value { get; set; } = string.Empty;
        public int notified_staff_id { get; set; }
        public string status { get; set; } = string.Empty;
    }
}
