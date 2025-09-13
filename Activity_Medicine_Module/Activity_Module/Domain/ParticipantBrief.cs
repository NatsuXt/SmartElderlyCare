namespace ElderCare.Api.Modules.Activities
{
    /// <summary>
    /// 活动参与者简要信息（用于 GET 返回）
    /// </summary>
    public sealed class ParticipantBrief
    {
        public int elderly_id { get; set; }
        public string elderly_name { get; set; } = string.Empty;
    }
}
