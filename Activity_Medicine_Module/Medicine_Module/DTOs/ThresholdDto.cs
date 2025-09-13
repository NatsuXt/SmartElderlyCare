namespace ElderCare.Api.Modules.Medical
{
    public sealed class ThresholdUpsertDto
    {
        public int elderly_id { get; set; }
        public string data_type { get; set; } = default!; // 如: "心率"/"HR"/"BP"
        public decimal? min_value { get; set; }           // 允许只给上限或下限
        public decimal? max_value { get; set; }
        public string? description { get; set; }
    }

}
