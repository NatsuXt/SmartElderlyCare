namespace ElderCare.Api.Modules.Medical
{
    public sealed class UpdateReminderStatusDto
    {
        public string status { get; set; } = string.Empty; // 例：未提醒 / 已提醒
    }
}
