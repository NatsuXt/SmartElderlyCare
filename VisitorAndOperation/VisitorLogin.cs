using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElderlyCareSystem.Models
{
    [Table("VisitorLogin")]
    public class VisitorLogin
    {
        [Key]
        [Column("VISITOR_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VisitorId { get; set; }

        [Required]
        [Column("VISITOR_PASSWORD")]
        [MaxLength(100)]
        public string VisitorPassword { get; set; } = string.Empty;

        [Required]
        [Column("VISITOR_PHONE")]
        [MaxLength(20)]
        [Phone]
        public string VisitorPhone { get; set; } = string.Empty;

        [Required]
        [Column("VISITOR_NAME")]
        [MaxLength(50)]
        public string VisitorName { get; set; } = string.Empty;
    }
}
