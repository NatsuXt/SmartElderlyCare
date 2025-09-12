using System.ComponentModel.DataAnnotations;

namespace Staff_Info.Models;

public class ROOMMANAGEMENT
{
    [Key]public decimal ROOM_ID { get; set; }
    public string ROOM_NUMBER { get; set; }
    public string ROOM_TYPE { get; set; }
    public decimal CAPACITY { get; set; }
    public string STATUS { get; set; }
    public decimal RATE { get; set; }
    public string BED_TYPE { get; set; }
    public decimal FLOOR { get; set; }
}