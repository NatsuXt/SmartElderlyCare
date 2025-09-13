namespace ElderCare.Api.Modules.Medical
{
    public sealed class VoiceAssistantReminder
    {
        public int reminder_id { get; set; }
        public int order_id { get; set; }
        public int elderly_id { get; set; }
        public DateTime reminder_time { get; set; }
        public int reminder_count { get; set; }
        public string reminder_status { get; set; } = string.Empty;
    }
}
