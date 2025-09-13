namespace ElderCare.Api.Modules.Medical
{
    public sealed class StockAggregate
    {
        public int medicine_id { get; set; }
        public int total_quantity { get; set; }
        public int reserved_quantity { get; set; }
        public int available_quantity { get; set; }
        public int active_batches { get; set; }
    }
}
