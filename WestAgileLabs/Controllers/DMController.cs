using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using System.Dynamic;
using WestAgileLabs.Data;
using WestAgileLabs.Models;

namespace WestAgileLabs.Controllers
{
    public class DMController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DMController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Home(LoginUser obj)
        {
            if (obj == null)
            {
                Console.WriteLine("emil is empty");
            }
            //Console.WriteLine("came to Get Home with Email : ", obj.Email);
            Employee emp = new Employee();
            var Employees = _db.Employees;
            var EmployeeRole = _db.EmployeeRoles;
            var Roles = _db.Roles;
            dynamic dynamicModel = new ExpandoObject();
            dynamicModel.Employees = Employees;
            dynamicModel.EmployeeRole = EmployeeRole;
            dynamicModel.Roles = Roles;
            dynamicModel.User = obj;
            return View(dynamicModel);
        }




        public IActionResult SearchEmployee()
        {
            int id = SkillId.Id;
            IEnumerable<Skill> skills = _db.Skills;
            var values = new ArrayList();
            if (id != 0)
            {
                string skillname = string.Empty;
                IEnumerable<Employee> employees = _db.Employees;
                IEnumerable<EmployeeSkill> employeeSkills = _db.EmployeeSkills;
                foreach (var item in skills)
                {
                    if (item.Id == id)
                    {
                        skillname = item.SkillName;
                    }
                }
                foreach (EmployeeSkill Empskill in employeeSkills)
                {
                    if (Empskill.SkillId == id)
                    {
                        Table t = new Table();
                        foreach (Employee emp in employees)
                        {
                            if (emp.Id == Empskill.EmployeeId)
                            {
                                t.Eid = emp.Id;
                                t.Email = emp.Email;
                                t.Name = emp.Employee_Name;
                                t.Skill = skillname;
                                t.Exp = Empskill.SkillExp;
                                values.Add(t);
                            }
                        }
                    }
                }
            }
            dynamic dynamicModel = new ExpandoObject();
            dynamicModel.skills = skills;
            dynamicModel.table = values;
            dynamicModel.idvalue = id;
            dynamicModel.roleid = 0;
            return View(dynamicModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchEmployee(string sid)
        {
            SkillId.Id = Convert.ToInt32(sid);
            return RedirectToAction("SearchEmployee", "DM");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignRole(string EIdRId)
        {
            string[] Email_RId = EIdRId.Split("~~");
            Console.WriteLine(Email_RId[0]);
            Console.WriteLine(Email_RId[1]);
            if (Email_RId.Length != 2 || Email_RId[0] == "admin@gmail.com")
            {
                return RedirectToAction("SearchEmployee");
            }
            else
            {
                int empid = 0;
                var emp = _db.Employees;
                foreach (var item in emp)
                {
                    if (item.Email == Email_RId[0])
                    {
                        empid = item.Id; break;
                    }
                }
                var emprole = _db.EmployeeRoles.Where(p => p.EmployeeId == empid);
                foreach (EmployeeRole role in emprole)
                {
                    role.RoleId = Convert.ToInt32(Email_RId[1]);
                    if (ModelState.IsValid)
                    {
                        _db.EmployeeRoles.Update(role);
                        _db.SaveChanges();
                        //Console.WriteLine("Role changed successfully");
                        return RedirectToAction("SearchEmployee");
                    }
                    //Console.WriteLine("not changed");
                }
            }
            return RedirectToAction("SearchEmployee");
        }
    }

    public class Table
    {
        public int Eid { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Skill { get; set; }
        public int Exp { get; set; }
    }

    public static class SkillId
    {
        private static int id;
        public static int Id
        {
            get { return id; }
            set { id = value; }
        }
    }
}
