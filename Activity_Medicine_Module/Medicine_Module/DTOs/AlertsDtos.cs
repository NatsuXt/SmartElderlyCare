using System;

namespace ElderCare.Api.Modules.Medical
{
    // 查询返回使用
    public sealed class AlertRecord
    {
        public int ALERT_ID { get; set; }
        public int ELDERLY_ID { get; set; }
        public string ALERT_TYPE { get; set; } = string.Empty;
        public DateTime ALERT_TIME { get; set; }
        public string ALERT_VALUE { get; set; } = string.Empty;      // 字段是 VARCHAR2(50)，因此用 string
        public int? NOTIFIED_STAFF_ID { get; set; }
        public string STATUS { get; set; } = string.Empty;           // OPEN/CLOSED 等
    }

    // GET 列表筛选
    public sealed class AlertsQuery
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 20;
        public int? elderly_id { get; set; }
        public string? alert_type { get; set; }
        public string? status { get; set; }   // OPEN / CLOSED
        public DateTime? from { get; set; }
        public DateTime? to { get; set; }
    }

    // POST /check 的入参
    public sealed class HealthDataSampleDto
    {
        public int elderly_id { get; set; }
        public string alert_type { get; set; } = string.Empty;
        public decimal value { get; set; }
        public DateTime? record_time { get; set; }          
        public int? notified_staff_id { get; set; }         
    }
}
