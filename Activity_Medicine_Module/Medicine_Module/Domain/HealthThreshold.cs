namespace ElderCare.Api.Modules.Medical
{
    public sealed class HealthThreshold
    {
        public int threshold_id { get; set; }
        public int elderly_id { get; set; }
        public string data_type { get; set; } = string.Empty;
        public float min_value { get; set; }
        public float max_value { get; set; }
        public string? description { get; set; }
    }
}
