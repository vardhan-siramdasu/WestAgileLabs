using System.ComponentModel.DataAnnotations;

namespace WestAgileLabs.Models
{
    public class EmployeeSkill
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int SkillId { get; set; }
        [Required]
        public int SkillExp { get; set; }
    }
}
