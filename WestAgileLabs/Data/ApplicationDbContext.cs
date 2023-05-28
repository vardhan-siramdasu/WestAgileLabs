using Microsoft.EntityFrameworkCore;
using WestAgileLabs.Models;

namespace WestAgileLabs.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Login> Logins { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<EmployeeRole> EmployeeRoles { get; set; }

        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
    }
}
