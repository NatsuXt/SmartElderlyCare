using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ELDERLYINFO")]
public class ElderlyInfo
{
    [Key]
    [Column("ELDERLY_ID")]
    public int ElderlyId { get; set; }

    [Required]
    [Column("NAME"), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("GENDER"), MaxLength(10)]
    public string Gender { get; set; } = string.Empty;

    [Column("BIRTH_DATE")]
    public DateTime? BirthDate { get; set; }

    [Column("ID_CARD_NUMBER"), MaxLength(18)]
    public string IdCardNumber { get; set; } = string.Empty;

    [Column("CONTACT_PHONE"), MaxLength(20)]
    public string ContactPhone { get; set; } = string.Empty;

    [Column("ADDRESS"), MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [Column("EMERGENCY_CONTACT"), MaxLength(200)]
    public string EmergencyContact { get; set; } = string.Empty;

    [Column("ADMISSION_DATE")]
    public DateTime? AdmissionDate { get; set; }

    [Column("ROOM_NUMBER"), MaxLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Column("CARE_LEVEL"), MaxLength(50)]
    public string CareLevel { get; set; } = string.Empty;

    [Column("HEALTH_STATUS"), MaxLength(50)]
    public string HealthStatus { get; set; } = string.Empty;

    [Column("STATUS"), MaxLength(20)]
    public string Status { get; set; } = "在院";

    // 导航属性（关联实体）
    public virtual ICollection<FamilyInfo> Families { get; set; } = new List<FamilyInfo>();
    public virtual ICollection<HealthMonitoring> HealthMonitorings { get; set; } = new List<HealthMonitoring>();
    public virtual ICollection<FeeSettlement> FeeSettlements { get; set; } = new List<FeeSettlement>();
    public virtual ICollection<VisitorRegistration> VisitorRegistrations { get; set; } = new List<VisitorRegistration>();
}