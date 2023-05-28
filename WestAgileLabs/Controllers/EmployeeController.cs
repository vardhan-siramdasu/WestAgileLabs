using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Dynamic;
using WestAgileLabs.Data;
using WestAgileLabs.Models;

namespace WestAgileLabs.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public EmployeeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Home(LoginUser obj)
        {
            Console.WriteLine("came to employee Home");
            Console.WriteLine("{0},{1},{2}",obj.Id,obj.Email,obj.Role);
            var emps = _db.Employees;
            var emprole = _db.EmployeeRoles;
            var role = _db.Roles;
            dynamic dynamicModel = new ExpandoObject();
            dynamicModel.Employees = emps;
            dynamicModel.EmployeeRole = emprole;
            dynamicModel.Roles = role;
            dynamicModel.User = obj;
            return View(dynamicModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Home()
        {
            return View();
        }

        public IActionResult Index(Login obj)
        {
            if (obj == null)
            {
                return NotFound();
            }
            SecondaryLogin.Login = obj;
            IEnumerable<Employee> EmployeeFromDb = _db.Employees.Where(p => p.Email == obj.UserName);
            if (EmployeeFromDb != null)
            {
                var skills = new ArrayList();
                var skillsexp = new ArrayList();
                foreach (Employee emp in EmployeeFromDb)
                {
                    //ViewData["EmployeeId"] = emp.Id;
                    var skillId = _db.EmployeeSkills.Where(q => q.EmployeeId == emp.Id);
                    var roleId = _db.EmployeeRoles.Where(q => q.EmployeeId == emp.Id);

                    dynamic dynamicModel = new ExpandoObject();

                    dynamicModel.Employee = EmployeeFromDb;

                    foreach (EmployeeSkill i in skillId)
                    {
                        var eachskill = _db.Skills.Where(p => p.Id == i.SkillId);
                        skillsexp.Add(i.SkillExp);
                        foreach (Skill item in eachskill)
                        {
                            skills.Add(item);
                        }
                    }
                    dynamicModel.Skillexp = skillsexp;
                    dynamicModel.EmployeeSkill = skills;

                    foreach (EmployeeRole i in roleId)
                    {
                        dynamicModel.EmployeeRole = _db.Roles.Where(p => p.Id == i.RoleId);
                    }

                    var masterSkills = new ArrayList();
                    IEnumerable<Skill> MasterSkills = _db.Skills;
                    foreach (Skill item in MasterSkills)
                    {
                        if (!skills.Contains(item))
                        {
                            masterSkills.Add(item);
                        }
                    }
                    dynamicModel.masterSkills = masterSkills;
                    return View(dynamicModel);
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSkilltoDB(string skills)
        {
            Console.WriteLine(skills);
            string[] skills_exp = skills.Split("~~or~~");
            string[] skill = skills_exp[0].Split("~~and~~");
            string[] exp = skills_exp[1].Split("~~and~~");
            Login login1 = SecondaryLogin.Login;
            int empid = 0;
            var emp = _db.Employees.Where(p => p.Email == login1.UserName);
            foreach (var item1 in emp)
            {
                empid = item1.Id;
                break;
            }
            for (int i = 0; i < skill.Length; i++)
            {
                int sid = Convert.ToInt32(skill[i]);
                int sexp = Convert.ToInt32(exp[i]);

                EmployeeSkill empSkill = new EmployeeSkill();
                empSkill.EmployeeId = empid;
                empSkill.SkillId = sid;
                empSkill.SkillExp = sexp;

                _db.EmployeeSkills.Add(empSkill);
                _db.SaveChanges();
                Console.WriteLine("skills added");
            }
            return RedirectToAction("Index", "Employee", login1);
        }
    }

    public static class SecondaryLogin
    {
        private static Login login = new Login();
        public static Login Login
        {
            get { return login; }
            set { login = value; }
        }
    }
}
