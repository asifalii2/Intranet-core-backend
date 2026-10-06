using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intranet.Domain.Entities
{
    /// <summary>
    /// Maps to existing Intranet_HR.Login table.
    /// </summary>
    [Table("Login")]
    public class Login
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("EmployeeID")]
        [MaxLength(50)]
        public string? EmployeeId { get; set; }

        [Column("LoginID")]
        [MaxLength(8)]
        public string? LoginId { get; set; }

        [Column("Name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column("Gender")]
        [MaxLength(1)]
        public string? Gender { get; set; }

        [Column("Designation")]
        [MaxLength(150)]
        public string? Designation { get; set; }

        [Column("Department")]
        [MaxLength(50)]
        public string? Department { get; set; }

        [Column("Location")]
        [MaxLength(50)]
        public string? Location { get; set; }

        [Column("Password")]
        [MaxLength(15)]
        public string? Password { get; set; }

        [Column("Status")]
        [MaxLength(50)]
        public string? Status { get; set; }

        [Column("Role")]
        [MaxLength(15)]
        public string? Role { get; set; }

        [Column("date")]
        public DateTime? Date { get; set; }

        [Column("LastLoginDate")]
        public DateTime? LastLoginDate { get; set; }
    }
}
