using System.ComponentModel.DataAnnotations;

namespace WestAgileLabs.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Employee_Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        [Required]
        public int Experience { get; set; }
    }
}
