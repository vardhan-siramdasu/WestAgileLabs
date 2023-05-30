using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data;
using System.Dynamic;
using WestAgileLabs.Data;
using WestAgileLabs.Models;

namespace WestAgileLabs.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Home(LoginUser obj)
        {
            if(obj.Email == null)
            {
                Console.WriteLine("emil is empty");
            }
            //Console.WriteLine("came to Get Home with Email : ",obj.Email);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Home(string Email)
        {
            //Console.WriteLine("came to post...");
            LoginUser obj = new LoginUser();
            obj.Email = Email;
            obj.Id = 1;
            obj.Role = "Admin";
            return RedirectToAction("Home", "Admin", obj);
        }

        public IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEmployee(Employee obj)
        {
            if (obj == null)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                bool value = _db.Employees.Contains(obj);
                if (value)
                {
                    return View();//employee already exist
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var EmpFromDB = _db.Employees.Where(p => p.Email == obj.Email);
                        if (EmpFromDB.Any())
                        {
                            return RedirectToAction("AddEmployee");
                        }
                        else
                        {
                            _db.Employees.Add(obj);
                            _db.SaveChanges();
                            //Console.WriteLine("added successfully");
                            var emp = _db.Employees.FirstOrDefault(p => p.Email == obj.Email);
                            EmployeeRole EmpRole = new EmployeeRole();
                            EmpRole.EmployeeId = emp.Id;
                            EmpRole.RoleId = 7;
                            EmpRole.EmployeeEmail = emp.Email;

                            _db.EmployeeRoles.Add(EmpRole);
                            _db.SaveChanges();
                            return RedirectToAction("AddEmployee");

                        }
                    }
                }
            }
            return RedirectToAction("AddEmployee");
        }

        public IActionResult AddMasterList()
        {
            var skills = _db.Skills;
            Console.WriteLine(skills.Any());
            dynamic dynamicModel = new ExpandoObject();
            dynamicModel.skills = skills;
            return View(dynamicModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMasterList(String SkillName)
        {
            Skill obj = new Skill();
            obj.SkillName = SkillName;
            Console.WriteLine(SkillName);
            if (obj == null)
            {
                return RedirectToAction("AddMasterList");
            }
            if (ModelState.IsValid)
            {
                bool value = _db.Skills.Contains(obj);
                if (value)
                {
                    //Console.WriteLine("already have");
                    return RedirectToAction("AddMasterList");//skill already exist
                }
                else
                {
                    var skills = _db.Skills;
                    foreach (var skill in skills)
                    {
                        if (skill.SkillName.Equals(obj.SkillName))
                        {
                            //Console.WriteLine("already exist");
                            return RedirectToAction("AddMasterList");
                        }
                    }
                    _db.Skills.Add(obj);
                    _db.SaveChanges();
                    //Console.WriteLine("added to list");
                    return RedirectToAction("AddMasterList");
                }
            }
            return RedirectToAction("AddMasterList");
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
            return RedirectToAction("SearchEmployee", "Admin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignRole(string EIdRId)
        {
            string[] Email_RId = EIdRId.Split("~~");
            //Console.WriteLine(Email_RId[0]);
            //Console.WriteLine(Email_RId[1]);
            if (Email_RId.Length != 2 || Email_RId[0] == "vardhan@gmail.com")
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

                var emprole = _db.EmployeeRoles.FirstOrDefault(p => p.EmployeeId == empid);
                emprole.RoleId = Convert.ToInt32(Email_RId[1]);
                if (ModelState.IsValid)
                {
                    _db.EmployeeRoles.Update(emprole);
                    _db.SaveChanges();
                    return RedirectToAction("SearchEmployee");
                }

            }
            return RedirectToAction("SearchEmployee");
        }

    }
}
