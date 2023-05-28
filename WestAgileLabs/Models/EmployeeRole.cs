using System.ComponentModel.DataAnnotations;

namespace WestAgileLabs.Models
{
    public class EmployeeRole
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public string EmployeeEmail { get; set; }
    }
}
