public class MedicineInventory
{
    public int MedicineId { get; set; }
    public string MedicineName { get; set; }
    public string MedicineType { get; set; }
    public decimal UnitPrice { get; set; }
    public int QuantityInStock { get; set; }
    public int MinimumStockLevel { get; set; }
    public string Supplier { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Description { get; set; }
}
