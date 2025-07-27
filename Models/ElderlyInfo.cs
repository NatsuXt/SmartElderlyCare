using System;
using System.ComponentModel.DataAnnotations;

namespace RoomDeviceManagement.Models
{
    /// <summary>
    /// 老人信息实体类
    /// </summary>
    public class ElderlyInfo
    {
        /// <summary>
        /// 老人ID（主键）
        /// </summary>
        [Key]
        public int ElderlyId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Required]
        [StringLength(10)]
        public string? Gender { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Required]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [Required]
        [StringLength(18)]
        public string? IdCardNumber { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [StringLength(20)]
        public string? ContactPhone { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        [StringLength(200)]
        public string? Address { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        [StringLength(200)]
        public string? EmergencyContact { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedTime { get; set; } = DateTime.Now;
    }
}
