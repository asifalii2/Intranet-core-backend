using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Domain.Entities
{
    /// <summary>
    /// Maps to existing Intranet_HR.LoginHistory table.
    /// </summary>
    [Table("LoginHistory")]
    public class LoginHistory
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("EmployeeID")]
        [MaxLength(50)]
        public string? EmployeeId { get; set; }

        [Column("LoginID")]
        [MaxLength(50)]
        public string? LoginId { get; set; }

        [Column("LoginDate")]
        public DateTime? LoginDate { get; set; }

        [Column("LoginTime")]
        public DateTime? LoginTime { get; set; }

        [Column("LogOutDate")]
        public DateTime? LogOutDate { get; set; }

        [Column("LogOutTime")]
        public DateTime? LogOutTime { get; set; }

        [Column("EventType")]
        [MaxLength(50)]
        public string? EventType { get; set; }

        [Column("IPAddress")]
        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [Column("UserAgent")]
        public string? UserAgent { get; set; }

        [Column("Browser")]
        [MaxLength(100)]
        public string? Browser { get; set; }

        [Column("OS")]
        [MaxLength(100)]
        public string? Os { get; set; }

        [Column("Device")]
        [MaxLength(50)]
        public string? Device { get; set; }
    }
}
