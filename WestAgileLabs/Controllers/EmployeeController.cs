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
            Console.WriteLine("{0},{1},{2}", obj.Id, obj.Email, obj.Role);
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
}
