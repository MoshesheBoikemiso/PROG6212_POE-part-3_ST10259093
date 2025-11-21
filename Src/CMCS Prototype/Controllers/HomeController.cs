using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Data;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserID);
                HttpContext.Session.SetString("UserRole", user.UserRole);
                HttpContext.Session.SetString("UserName", user.FirstName);

                if (user.UserRole == "HR")
                {
                    return RedirectToAction("HRDashboard", "HR");
                }
                else if (user.UserRole == "Lecturer")
                {
                    return RedirectToAction("LecturerDashboard", "Claim");
                }
                else if (user.UserRole == "Coordinator" || user.UserRole == "Manager")
                {
                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "User not found. Try: lecturer@email.com, coordinator@email.com, manager@email.com, or hr@email.com";
            return View("Index");
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == "HR")
                return RedirectToAction("HRDashboard", "HR");
            else if (userRole == "Lecturer")
                return RedirectToAction("LecturerDashboard", "Claim");
            else if (userRole == "Coordinator" || userRole == "Manager")
                return RedirectToAction("AdminDashboard", "Admin");
            else
                return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult CreateHRUser()
        {
            if (!_context.Users.Any(u => u.UserRole == "HR"))
            {
                var hrUser = new User
                {
                    FirstName = "HR",
                    LastName = "Manager",
                    Email = "hr@email.com",
                    UserRole = "HR"
                };

                _context.Users.Add(hrUser);
                _context.SaveChanges();

                return Content("HR user created successfully! Email: hr@email.com");
            }

            return Content("HR user already exists!");
        }

        public IActionResult GenerateDatabaseScript()
        {
            try
            {
                var sql = _context.Database.GenerateCreateScript();

                var bytes = System.Text.Encoding.UTF8.GetBytes(sql);
                return File(bytes, "text/plain", "CMCS-Database-Schema.sql");
            }
            catch (Exception ex)
            {
                return Content($"Error generating script: {ex.Message}");
            }
        }
    }
}