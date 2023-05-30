using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;
using WestAgileLabs.Data;
using WestAgileLabs.Models;


namespace WestAgileLabs.Controllers
{

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LoginUser loginUser = new LoginUser();
        
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Home()
        {
            SecondaryLoginUser.Loginuser = new LoginUser();
            var Employees = _db.Employees;
            var EmployeeRole = _db.EmployeeRoles;
            var Roles = _db.Roles;
            dynamic dynamicModel = new ExpandoObject();
            dynamicModel.Employees = Employees;
            dynamicModel.EmployeeRole = EmployeeRole;
            dynamicModel.Roles = Roles;
            return View(dynamicModel);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login obj, string Login)
        {
            if (obj == null)
            {
                return View();
            }
            //Console.WriteLine(Login);
            if (ModelState.IsValid)
            {
                if (Login == "Sign In")
                {
                    var UserFromDb = _db.Logins.Find(obj.UserName);
                    if (UserFromDb == null)
                    {
                        ViewData["ValidPassword"] = "false";
                        return View();
                    }
                    var hashpassword = getHash(obj.Password);
                    if (hashpassword == UserFromDb.Password)
                    {
                        var UserFromLoginRole = _db.EmployeeRoles.FirstOrDefault(p => p.EmployeeEmail == obj.UserName);
                        string role;
                        var r = _db.Roles.FirstOrDefault(p => p.Id == UserFromLoginRole.RoleId);
                        loginUser.Role = r.RoleName.ToString();
                        loginUser.Email = obj.UserName.ToString();
                        loginUser.Id = Convert.ToInt32(UserFromLoginRole.EmployeeId);
                        role = UserFromLoginRole.RoleId.ToString();
                        SecondaryLoginUser.Loginuser = loginUser;
                        if (role == "1")
                            return RedirectToAction("Home", "Admin", loginUser);
                        else if (role == "2")
                            return RedirectToAction("Home", "HR", loginUser);
                        else if (role == "3")
                            return RedirectToAction("Home", "TM", loginUser);
                        else if (role == "4")
                            return RedirectToAction("Home", "DM", loginUser);
                        else if (Convert.ToInt32(role) >= 5)
                            return RedirectToAction("Home", "Employee", loginUser);
                        else
                            Console.WriteLine("role not found");
                        return RedirectToAction("Home", "Employee", loginUser);

                    }
                    else
                    {
                        ViewData["ValidPassword"] = "false";
                    }
                    return View();
                }
                else if (Login == "Sign Up")
                {
                    var UserFromDbEmp = _db.Employees.Where(p => p.Email == obj.UserName).ToList();
                    if (UserFromDbEmp.Any())
                    {
                        var UserFromDbLogin = _db.Logins.Where(p => p.UserName == obj.UserName).ToList();
                        if (UserFromDbLogin.Any())
                        {
                            ViewData["ExistingUser"] = "true";
                            return View();
                        }
                        else
                        {
                            obj.Password = getHash(obj.Password);
                            _db.Logins.Add(obj);
                            ViewData["UserCreated"] = "true";
                            _db.SaveChanges();
                            return View();
                        }
                    }
                    else
                    {
                        ViewData["UnauthoriredUser"] = "true";
                        return View();
                    }
                }
            }
            return View();
        }

        public IActionResult Profile(int id)
        {
            //Console.WriteLine("came to Profile Action with EID :{0} ", id);
            //Console.WriteLine("{0},{1},{2}", loginUser.Id, loginUser.Email, loginUser.Role);
            IEnumerable<Employee> EmployeeFromDb = _db.Employees.Where(p => p.Id == id);

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
                dynamicModel.LoginUser = loginUser;
                return View(dynamicModel);
            }
            return View();
        }


        private static string getHash(string text)
        {
            // SHA512 is disposable by inheritance.  
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                // Get the hashed string.  
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public IActionResult SelfProfile(LoginUser obj)
        {
            if (obj == null)
            {
                return NotFound();
            }

            //Console.WriteLine("{0}",obj.Email);
            //Console.WriteLine("{0}",SecondaryLoginUser.Loginuser.Email);
            SecondaryLoginUser.Loginuser = obj;
            IEnumerable<Employee> EmployeeFromDb = _db.Employees.Where(p => p.Email == obj.Email);
            if (EmployeeFromDb != null)
            {
                var skills = new ArrayList();
                var skillsexp = new ArrayList();
                foreach (Employee emp in EmployeeFromDb)
                {
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
                    //Console.WriteLine("send to profile page");
                    return View(dynamicModel);
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSkilltoDB(string skills)
        {
            //Console.WriteLine(skills);
            string[] skills_exp = skills.Split("~~or~~");
            string[] skill = skills_exp[0].Split("~~and~~");
            string[] exp = skills_exp[1].Split("~~and~~");
            LoginUser login1 = SecondaryLoginUser.Loginuser;
            int empid = 0;
            var emp = _db.Employees.Where(p => p.Email == login1.Email);
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
                //Console.WriteLine("skills added");
            }
            return RedirectToAction("SelfProfile", "Home", login1);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}