using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using WestAgileLabs.Data;
using WestAgileLabs.Models;

namespace WestAgileLabs.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HRController(ApplicationDbContext db)
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
    }
}
