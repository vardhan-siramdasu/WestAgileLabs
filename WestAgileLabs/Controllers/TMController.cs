using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using WestAgileLabs.Data;
using WestAgileLabs.Models;

namespace WestAgileLabs.Controllers
{
    public class TMController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TMController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Home(LoginUser obj)
        {
            //Console.WriteLine("came to TM Home");
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
    }
}
