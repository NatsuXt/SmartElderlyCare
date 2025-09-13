using System.ComponentModel.DataAnnotations;

namespace ElderCare.Api.Modules.Medical
{
    /// <summary>
    /// 新建提醒的请求体（状态不需要传，固定为“待提醒”）
    /// </summary>
    public sealed class CreateReminderDto
    {
        [Required] public int order_id { get; set; }
        [Required] public int elderly_id { get; set; }
        [Required] public DateTime reminder_time { get; set; }
        [Range(1, int.MaxValue)] public int reminder_count { get; set; } = 1;
    }

    /// <summary>
    /// 列表返回的提醒项
    /// </summary>
    public sealed class ReminderItemDto
    {
        public int reminder_id { get; set; }
        public int order_id { get; set; }
        public int elderly_id { get; set; }
        public DateTime reminder_time { get; set; }
        public int reminder_count { get; set; }
        public string reminder_status { get; set; } = "";
    }
}
