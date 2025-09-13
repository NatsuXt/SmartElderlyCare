using System;

namespace ElderCare.Api.Modules.Activities
{
    /// <summary>
    /// 报名/签到/取消 的请求键
    /// </summary>
    public sealed class ParticipationKeyDto
    {
        public int activity_id { get; set; }
        public int elderly_id { get; set; }
    }

    /// <summary>
    /// GET: 按活动查询返回项
    /// - 若活动“报名中” => 返回 status=已报名 的记录
    /// - 若活动“已结束” => 返回 status=已参加 的记录
    /// </summary>
    public sealed class ActivityParticipationItemDto
    {
        public int participation_id { get; set; }
        public int activity_id { get; set; }
        public int elderly_id { get; set; }
        public string elderly_name { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;            // 已报名 / 已参加
        public DateTime registration_time { get; set; }
        public DateTime? check_in_time { get; set; }
    }

    /// <summary>
    /// GET: 按老人查询返回项
    /// - display_status：若活动已结束且未参加 => 缺席（派生显示，不落库）
    /// </summary>
    public sealed class ElderlyParticipationItemDto
    {
        public int participation_id { get; set; }
        public int activity_id { get; set; }
        public string activity_name { get; set; } = string.Empty;
        public DateTime activity_date { get; set; }
        public string activity_time { get; set; } = string.Empty;     // HH:mm:ss.FFFFFF（由 TO_CHAR 转换）
        public string location { get; set; } = string.Empty;

        public string raw_status { get; set; } = string.Empty;        // 原始：已报名/已参加
        public string display_status { get; set; } = string.Empty;    // 派生：缺席/已报名/已参加

        public DateTime registration_time { get; set; }
        public DateTime? check_in_time { get; set; }
    }
}
