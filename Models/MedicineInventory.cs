using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class MEDICINEINVENTORY
{
    [Key]public decimal MEDICINE_ID { get; set; }
    public string MEDICINE_NAME { get; set; }
    public string MEDICINE_TYPE { get; set; }
    public decimal UNIT_PRICE { get; set; }
    public int QUANTITY_IN_STOCK { get; set; }
    public int MINIMUM_STOCK_LEVEL { get; set; }
    public string SUPPLIER { get; set; }
    public DateTime EXPIRATION_DATE { get; set; }
    public string DESCRIPTION { get; set; }

    // 反向导航属性（被哪些表引用）
    public ICollection<MEDICALORDER> MEDICALORDERS { get; set; }
}