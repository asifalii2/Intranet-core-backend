using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Domain.Entities
{
    /// <summary>
    /// Maps to existing Intranet_HR.Employee table.
    /// Only columns required for auth/login are included for now.
    /// </summary>
    [Table("Employee")]
    public class Employee
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [Column("EmployeeID")]
        public string EmployeeId { get; set; } = string.Empty;

        [Column("EmployeeName")]
        [MaxLength(50)]
        public string? EmployeeName { get; set; }

        [Column("Location")]
        [MaxLength(50)]
        public string? Location { get; set; }

        [Column("Position")]
        [MaxLength(200)]
        public string? Position { get; set; }

        [Column("LoginID")]
        [MaxLength(10)]
        public string? LoginId { get; set; }

        [Column("Role")]
        [MaxLength(50)]
        public string? Role { get; set; }

        [Required]
        [Column("Global_ID")]
        [MaxLength(50)]
        public string GlobalId { get; set; } = string.Empty;

        [Column("status")]
        [MaxLength(15)]
        public string? Status { get; set; }
    }
}
