using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("STAFFINFO")]
public class StaffInfo
{
    [Key]
    [Column("STAFF_ID")]
    public int staff_id { get; set; }

    [Required]
    [Column("NAME"), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("GENDER"), MaxLength(10)]
    public string Gender { get; set; } = string.Empty;

    [Column("BIRTH_DATE")]
    public DateTime? BirthDate { get; set; }

    [Column("ID_CARD_NUMBER"), MaxLength(18)]
    public string IdCardNumber { get; set; } = string.Empty;

    [Column("PHONE"), MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Column("EMAIL"), MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Column("POSITION"), MaxLength(50)]
    public string Position { get; set; } = string.Empty;

    [Column("DEPARTMENT"), MaxLength(50)]
    public string Department { get; set; } = string.Empty;

    [Column("HIRE_DATE")]
    public DateTime? HireDate { get; set; }

    [Column("STATUS"), MaxLength(20)]
    public string Status { get; set; } = "在职";

    [Column("ADDRESS"), MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [Column("EMERGENCY_CONTACT"), MaxLength(200)]
    public string EmergencyContact { get; set; } = string.Empty;

    [Column("SALARY")]
    public decimal? Salary { get; set; }

    [Column("WORK_SCHEDULE"), MaxLength(100)]
    public string WorkSchedule { get; set; } = string.Empty;
}