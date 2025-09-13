namespace ElderCare.Api.Modules.Activities
{
    public sealed class ActivityParticipation
    {
        public int participation_id { get; set; }
        public int activity_id { get; set; }
        public int elderly_id { get; set; }
        public string status { get; set; } = string.Empty; // 已报名 / 已参加 / 缺席
        public DateTime registration_time { get; set; }
        public DateTime? check_in_time { get; set; }
        public string? feedback { get; set; }
    }
}
